using NUnit.Framework;
using System.Collections.Generic;
using WebCompiler.Compile;
using WebCompiler.Configuration;
using WebCompiler.Configuration.Settings;

namespace Tests_WebCompiler
{
    public class CompileMinifyZipMoveCleanupTests : TestsBase
    {
        [SetUp]
        public void CreatePipeline()
        {
            pipeline = (file) => new CompilationStep(file)
                .With(new SassCompiler(new SassSettings()))
                .Then(new CssAutoprefixer(new CssAutoprefixSettings()))
                .Then(new CssMinifier(new CssMinifySettings { TermSemicolons = false }))
                .Then(new Zipper())
                .Then(new Place("../../../TestCases/", "../../../TestCases/Scss/"))
                .Then(new Cleaner());
            input = "../../../TestCases/Scss/test.scss";
            output_files = new List<string> { "../../../TestCases/test.min.css.gz" };
            expected_output = "../../../TestCases/GzCss/test.min.css.gz";
            DeleteTemporaryFiles();
        }
        [Test]
        public void CallTest() => Test();
    }
}