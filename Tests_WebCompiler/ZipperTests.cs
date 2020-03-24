using NUnit.Framework;
using System.Collections.Generic;
using WebCompiler.Compile;

namespace Tests_WebCompiler
{
    public class ZipperTests : TestsBase
    {
        [SetUp]
        public void CreatePipeline()
        {
            pipeline = (file) => new CompilationStep(file).With(new Zipper());
            input = "../../../TestCases/Css/test.css";
            output_files = new List<string> { "../../../TestCases/Css/test.css.gz" };
            expected_output = "../../../TestCases/GzCss/test.css.gz";
            DeleteTemporaryFiles();
        }
        [Test]
        public void CallTest() => Test();
    }
}