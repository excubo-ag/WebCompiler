using NUnit.Framework;
using System.Collections.Generic;
using WebCompiler.Compile;
using WebCompiler.Configuration;
using WebCompiler.Configuration.Settings;

namespace Tests_WebCompiler
{
    public class AutoPipelineNoGzipTests : TestsBase
    {
        [SetUp]
        public void CreatePipeline()
        {
            pipeline = file => new Compilers(new Config
            {
                Minifiers = new MinificationSettings
                {
                    Css = new CssMinifySettings
                    {
                        TermSemicolons = false
                    },
                    GZip = false
                }
            }, "../../../TestCases/Scss/").TryCompile(file);
            input = "../../../TestCases/Scss/test.scss";
            output_files = new List<string> { "../../../TestCases/Scss/test.css", "../../../TestCases/Scss/test.min.css" };
            expected_output = "../../../TestCases/MinCss/test.min.css";
            DeleteOutputFiles();
        }
        [Test]
        public void CallTest() => Test();
    }
}