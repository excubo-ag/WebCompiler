using NUnit.Framework;
using System.Collections.Generic;
using WebCompiler.Compile;
using WebCompiler.Configuration;
using WebCompiler.Configuration.Settings;

namespace Tests_WebCompiler
{
    public class ScssTests : TestsBase
    {
        [SetUp]
        public void CreatePipeline()
        {
            pipeline = (file) =>
                    new CompilationStep(file)
                           .With(new SassCompiler(new SassSettings()))
                           .Then(new CssAutoprefixer(new CssAutoprefixSettings()));
            input = "../../../TestCases/Scss/test.scss";
            output_files = new List<string> { "../../../TestCases/Scss/test.css" };
            expected_output = "../../../TestCases/Css/test.css";
            DeleteOutputFiles();
        }
        [Test]
        public void CallTest() => Test();
    }
}