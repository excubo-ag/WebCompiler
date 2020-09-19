using NUnit.Framework;
using System.Collections.Generic;
using WebCompiler.Compile;
using WebCompiler.Configuration;
using WebCompiler.Configuration.Settings;

namespace Tests_WebCompiler
{
    public class CompileMinifyTests : TestsBase
    {
        [SetUp]
        public void CreatePipeline()
        {
            pipeline = (file) => 
                    new CompilationStep(file)
                           .With(new SassCompiler(new SassSettings()))
                           .Then(new CssAutoprefixer(new CssAutoprefixSettings()))
                           .Then(new CssMinifier(new CssMinifySettings { TermSemicolons = false }));
            input = "../../../TestCases/Scss/test.scss";
            output_files = new List<string> { "../../../TestCases/Scss/test.css", "../../../TestCases/Scss/test.min.css" };
            expected_output = "../../../TestCases/MinCss/test.min.css";
            DeleteTemporaryFiles();
        }
        [Test]
        public void CallTest() => Test();
    }
}