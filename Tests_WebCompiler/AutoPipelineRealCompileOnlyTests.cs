using NUnit.Framework;
using System.Collections.Generic;
using WebCompiler.Compile;
using WebCompiler.Configuration;

namespace Tests_WebCompiler
{
    public class AutoPipelineRealCompileOnlyTests : TestsBase
    {
        [SetUp]
        public void CreatePipeline()
        {
            pipeline = file => new Compilers(new Config
            {
                Minifiers = new MinificationSettings
                {
                    Enabled = false,
                    GZip = false
                }
            }, "../../../TestCases/Scss/").TryCompile(file);
            input = "../../../TestCases/Scss/site.scss";
            output_files = new List<string> { "../../../TestCases/Scss/site.css" };
            expected_output = "../../../TestCases/Css/site.css";
        }
        [Test]
        public void CallTest() => Test();
    }
}