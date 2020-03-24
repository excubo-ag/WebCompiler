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
            Assert.AreEqual(timestamp, new_timestamp);
            TestEqual(expected_output, output_files.Last());
        }
        private DateTime ProcessFile()
        {
            CompilationStep result = null;
            Assert.DoesNotThrow(() => result = pipeline(input));
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Errors == null || !result.Errors.Any());
            Assert.AreEqual(Path.GetFullPath(output_files.Last()), Path.GetFullPath(result.OutputFile));
            TestEqual(expected_output, result.OutputFile);
            foreach (var file in output_files)
            {
                Assert.IsTrue(File.Exists(file));
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
                Assert.AreEqual(e, r);
            }
            else
            {
                Assert.AreEqual(File.ReadAllText(expected, Compiler.Encoding), File.ReadAllText(value, Compiler.Encoding));
            }
        }
    }
}