using NUnit.Framework;
using System.Collections.Generic;
using WebCompiler.Compile;
using WebCompiler.Configuration;
using WebCompiler.Configuration.Settings;

namespace Tests_WebCompiler
{
    public class CompileZipTests : TestsBase
    {
        [SetUp]
        public void CreatePipeline()
        {
            pipeline = (file) =>
                    new CompilationStep(file)
                           .With(new SassCompiler(new SassSettings()))
                           .Then(new CssAutoprefixer(new CssAutoprefixSettings()))
                           .Then(new Zipper());
            input = "../../../TestCases/Scss/test.scss";
            output_files = new List<string> { "../../../TestCases/Scss/test.css", "../../../TestCases/Scss/test.css.gz" };
            expected_output = "../../../TestCases/GzCss/test.css.gz";
            DeleteOutputFiles();
        }
        [Test]
        public void CallTest() => Test();
    }
}