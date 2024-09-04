using NUnit.Framework;
using System.Collections.Generic;
using WebCompiler.Compile;
using WebCompiler.Configuration;

namespace Tests_WebCompiler
{
    public class AutoPipelineRealNoGzipTests : TestsBase
    {
        [SetUp]
        public void CreatePipeline()
        {
            pipeline = file => new Compilers(new Config
            {
                Minifiers = new MinificationSettings
                {
                    GZip = false
                }
            }, "../../../TestCases/Scss/").TryCompile(file);
            input = "../../../TestCases/Scss/site.scss";
            output_files = new List<string> { "../../../TestCases/Scss/site.css", "../../../TestCases/Scss/site.min.css" };
            expected_output = "../../../TestCases/MinCss/site.min.css";
        }
        [Test]
        public void CallTest() => Test();
    }
}