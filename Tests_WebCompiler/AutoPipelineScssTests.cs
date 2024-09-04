using NUnit.Framework;
using System.Collections.Generic;
using WebCompiler.Compile;
using WebCompiler.Configuration;
using WebCompiler.Configuration.Settings;

namespace Tests_WebCompiler
{
    public class AutoPipelineScssTests : TestsBase
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
            }, "../../../TestCases/Scss/").TryCompile(file);
            input = "../../../TestCases/Scss/test.scss";
            output_files = new List<string> { "../../../TestCases/Scss/test.css", "../../../TestCases/Scss/test.min.css", "../../../TestCases/Scss/test.min.css.gz" };
            expected_output = "../../../TestCases/GzCss/test.min.css.gz";
            DeleteOutputFiles();
        }
        [Test]
        public void CallTest() => Test();
    }
}