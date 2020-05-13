using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using WebCompiler.Compile;
using WebCompiler.Configuration.Settings;

namespace Tests_WebCompiler
{
    public class RecompileTests : TestsBase
    {
        [SetUp]
        public void CreatePipeline()
        {
            pipeline = (file) => new CompilationStep(file).With(new SassCompiler(new SassSettings())).Then(new CssMinifier(new CssMinifySettings { TermSemicolons = false }));
            input = "../../../TestCases/Scss/test.scss";
            output_files = new List<string> { "../../../TestCases/Scss/test.css", "../../../TestCases/Scss/test.min.css" };
            expected_output = "../../../TestCases/MinCss/test.min.css";
            DeleteTemporaryFiles();
        }
        [Test]
        public void CallTest()
        {
            var timestamp = ProcessFile();
            File.Copy(input, input + ".bak");
            File.AppendAllText(input, "\n.new-rule { color: black; }");
            var new_timestamp = ProcessFile();
            File.Move(input + ".bak", input, overwrite: true);
            Assert.AreNotEqual(timestamp, new_timestamp, "Compiling a second time should alter the file, since there is an actual change for once!");
        }
    }
}