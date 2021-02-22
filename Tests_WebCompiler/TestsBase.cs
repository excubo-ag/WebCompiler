using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using WebCompiler.Compile;

namespace Tests_WebCompiler
{
    public class TestsBase
    {
        protected Func<string, CompilationStep> pipeline;
        protected string input;
        protected List<string> output_files;
        protected List<string> unexpected_files;
        protected string expected_output;
        [TearDown]
        protected void DeleteTemporaryFiles()
        {
            if (output_files == null)
            {
                return;
            }
            foreach (var file in output_files)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }
        protected void Test()
        {
            var timestamp = ProcessFile();
            var new_timestamp = ProcessFile();
            Assert.AreEqual(timestamp, new_timestamp, "Compiling a second time should not alter the file");
            TestEqual(expected_output, output_files.Last());
        }
        protected DateTime ProcessFile()
        {
            CompilationStep result = null;
            Assert.DoesNotThrow(() => result = pipeline(input), "Compiling should not result in exception");
            Assert.IsNotNull(result, "Compilation result may not be null");
            Assert.IsTrue(result.Errors == null || !result.Errors.Any(), "Compilation should not result in error");
            Assert.AreEqual(Path.GetFullPath(output_files.Last()), Path.GetFullPath(result.OutputFile), "Unexpected output file");
            foreach (var file in output_files)
            {
                Assert.IsTrue(File.Exists(file), $"Output file or intermediate file {file} should exist");
            }
            if (unexpected_files != null)
            {
                foreach (var file in unexpected_files)
                {
                    Assert.IsFalse(File.Exists(file), $"File {file} should NOT exist, but does");
                }
            }
            return new FileInfo(output_files.Last()).LastWriteTimeUtc;
        }
        private static string Decompressed(string file)
        {
            using var source_stream = File.OpenRead(file);
            using var gzip_stream = new GZipStream(source_stream, CompressionMode.Decompress);
            var stream_reader = new StreamReader(gzip_stream);
            return stream_reader.ReadToEnd();
        }
        protected void TestEqual(string expected, string value)
        {
            if (expected.EndsWith("gz"))
            {
                var normalisedExpected = Regex.Replace(Decompressed(expected), @"\r\n?|\n", string.Empty).Replace('\\', '/');
                var normalisedActual = Regex.Replace(Decompressed(value), @"\r\n?|\n", string.Empty).Replace('\\', '/');
                Assert.AreEqual(normalisedExpected, normalisedActual, "Compressed files should have the same content when decompressed");
            }
            else
            {
                var normalisedExpected = Regex.Replace(File.ReadAllText(expected, Compiler.Encoding), @"\r\n?|\n", string.Empty).Replace('\\', '/');
                var normalisedActual = Regex.Replace(File.ReadAllText(value, Compiler.Encoding), @"\r\n?|\n", string.Empty).Replace('\\', '/');
                Assert.AreEqual(normalisedExpected, normalisedActual, "Files should be identical");
            }
        }
    }
}