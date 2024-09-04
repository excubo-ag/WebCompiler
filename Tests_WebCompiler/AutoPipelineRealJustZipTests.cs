using NUnit.Framework;
using System.Collections.Generic;
using WebCompiler.Compile;
using WebCompiler.Configuration;
using WebCompiler.Configuration.Settings;

namespace Tests_WebCompiler
{
    public class AutoPipelineRealJustZipTests : TestsBase
    {
        [SetUp]
        public void CreatePipeline()
        {
            pipeline = file => new Compilers(new Config
            {
                Minifiers = new MinificationSettings
                {
                    GZip = true,
                    Css = new CssMinifySettings
                    {
                        TermSemicolons = false
                    }
                }
            }, "../../../TestCases/MinCss/").TryCompile(file);
            input = "../../../TestCases/MinCss/site.min.css";
            output_files = new List<string> { "../../../TestCases/MinCss/site.min.css.gz" };
            expected_output = "../../../TestCases/GzCss/site.min.css.gz";
            DeleteTemporaryFiles();
        }
        [Test]
        public void CallTest() => Test();
    }
}