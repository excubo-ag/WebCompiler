using NUnit.Framework;
using System.Collections.Generic;
using WebCompiler.Compile;
using WebCompiler.Configuration;
using WebCompiler.Configuration.Settings;

namespace Tests_WebCompiler
{
    public class AutoPipelineRealTests : TestsBase
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
                        TermSemicolons = true
                    }
                }
            }, "../../../TestCases/Scss/").TryCompile(file);
            input = "../../../TestCases/Scss/site.scss";
            output_files = new List<string> { "../../../TestCases/Scss/site.css", "../../../TestCases/Scss/site.min.css", "../../../TestCases/Scss/site.min.css.gz" };
            expected_output = "../../../TestCases/GzCss/site.min.css.gz";
            DeleteTemporaryFiles();
        }
        [Test]
        public void CallTest() => Test();
    }
}