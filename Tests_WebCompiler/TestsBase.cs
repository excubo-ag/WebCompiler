using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using WebCompiler.Compile;

namespace Tests_WebCompiler
{
    public class TestsBase
    {
        protected Func<string, CompilationStep> pipeline;
        protected string input;
        protected List<string> output_files;
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
                File.Delete(file);
            }
        }
        protected void Test()
        {
            var timestamp = ProcessFile();
            var new_timestamp = ProcessFile();
            Assert.AreEqual(timestamp, new_timestamp, "Compiling a second time should not alter the file");
            TestEqual(expected_output, output_files.Last());
        }
        private DateTime ProcessFile()
        {
            CompilationStep result = null;
            Assert.DoesNotThrow(() => result = pipeline(input), "Compiling should not result in exception");
            Assert.IsNotNull(result, "Compilation result may not be null");
            Assert.IsTrue(result.Errors == null || !result.Errors.Any(), "Compilation should not result in error");
            Assert.AreEqual(Path.GetFullPath(output_files.Last()), Path.GetFullPath(result.OutputFile), "Unexpected output file");
            TestEqual(expected_output, result.OutputFile);
            foreach (var file in output_files)
            {
                Assert.IsTrue(File.Exists(file), $"Output file or intermediate file {file} should exist");
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
        private void TestEqual(string expected, string value)
        {
            if (expected.EndsWith("gz"))
            {
                var e = Decompressed(expected);
                var r = Decompressed(value);
                Assert.AreEqual(e, r, "Compressed files should have the same content when decompressed");
            }
            else
            {
                Assert.AreEqual(File.ReadAllText(expected, Compiler.Encoding), File.ReadAllText(value, Compiler.Encoding), "Files should be identical");
            }
        }
    }
}