using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebCompiler.Compile;
using WebCompiler.Configuration.Settings;

namespace Tests_WebCompiler
{
    public class NoPreserveTests : TestsBase
    {
        [SetUp]
        public void CreatePipeline()
        {
            pipeline = (file) => new CompilationStep(file)
            .With(new SassCompiler(new SassSettings(), new WebCompiler.Configuration.CssAutoprefixSettings()))
            .Then(new CssMinifier(new CssMinifySettings { TermSemicolons = false }))
            .Then(new Place("../../../output/", "../../../TestCases/Scss/"))
            .Then(new Cleaner());
            input = "../../../TestCases/Scss/test.scss";
            output_files = new List<string> { "../../../output/test.min.css" };
            unexpected_files = new List<string> { "../../../TestCases/Scss/test.css", "../../../TestCases/Scss/test.min.css" };
            expected_output = "../../../TestCases/MinCss/test.min.css";
            DeleteTemporaryFiles();
        }
        /// <summary>
        /// Even if we do not preserve the intermediate files, the output file should only be changed, if the source file changed.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task CallNoRecompilation()
        {
            var timestamp = ProcessFile();
            //await Task.Delay(100); // create a delay, because if things happen fast enough, the accuracy of the file timestamp is too low to detect the change in file
            var new_timestamp = ProcessFile();
            Assert.AreEqual(timestamp, new_timestamp, "Compiling a second time shouldn't alter the file");
        }
        [Test]
        public async Task CallTest()
        {
            var timestamp = ProcessFile();
            await Task.Delay(100); // create a delay, because if things happen fast enough, the accuracy of the file timestamp is too low to detect the change in file
            File.Copy(input, input + ".bak");
            File.AppendAllText(input, "\n.new-rule { color: black; }");
            await Task.Delay(100); // create a delay, because if things happen fast enough, the accuracy of the file timestamp is too low to detect the change in file
            var new_timestamp = ProcessFile();
            File.Move(input + ".bak", input, overwrite: true);
            Assert.AreNotEqual(timestamp, new_timestamp, "Compiling a second time should alter the file, since there is an actual change for once!");
        }
        [Test]
        public async Task CallNeedsCompileSubDirTest()
        {
            var timestamp = ProcessFile();
            await Task.Delay(100); // create a delay, because if things happen fast enough, the accuracy of the file timestamp is too low to detect the change in file
            var input_path = new FileInfo(input).DirectoryName ?? string.Empty;
            // update the scss source file in a sub directory.
            var import_file = Path.Combine(input_path, "sub", "_bar.scss");
            File.Copy(import_file, import_file + ".bak", overwrite: true);
            File.AppendAllText(import_file, "\n.new-rule { color: black; }");
            await Task.Delay(100); // create a delay, because if things happen fast enough, the accuracy of the file timestamp is too low to detect the change in file
            var new_timestamp = ProcessFile();
            File.Move(import_file + ".bak", import_file, overwrite: true);
            Assert.AreNotEqual(timestamp, new_timestamp, "Compiling a second time should alter the file, since there is an actual change for once!");
        }
    }
}