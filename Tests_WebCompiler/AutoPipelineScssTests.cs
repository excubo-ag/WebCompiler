using NUnit.Framework;
using System.Collections.Generic;
using WebCompiler.Compile;

namespace Tests_WebCompiler
{
    public class AutoPipelineScssTests : TestsBase
    {
        [SetUp]
        public void CreatePipeline()
        {
            pipeline = (file) => new Compilers(new WebCompiler.Configuration.Config
            {
                Minifiers = new WebCompiler.Configuration.MinificationSettings
                {
                    Css = new WebCompiler.Configuration.Settings.CssMinifySettings
                    {
                        TermSemicolons = false
                    }
                }
            }, "../../../TestCases/Scss/").TryCompile(file);
            input = "../../../TestCases/Scss/test.scss";
            output_files = new List<string> { "../../../TestCases/Scss/test.css", "../../../TestCases/Scss/test.min.css", "../../../TestCases/Scss/test.min.css.gz" };
            expected_output = "../../../TestCases/GzCss/test.min.css.gz";
            DeleteTemporaryFiles();
        }
        [Test]
        public void CallTest() => Test();
    }
    public class AutoPipelineNoGzipTests : TestsBase
    {
        [SetUp]
        public void CreatePipeline()
        {
            pipeline = (file) => new Compilers(new WebCompiler.Configuration.Config
            {
                Minifiers = new WebCompiler.Configuration.MinificationSettings
                {
                    Css = new WebCompiler.Configuration.Settings.CssMinifySettings
                    {
                        TermSemicolons = false
                    },
                    GZip = false
                }
            }, "../../../TestCases/Scss/").TryCompile(file);
            input = "../../../TestCases/Scss/test.scss";
            output_files = new List<string> { "../../../TestCases/Scss/test.css", "../../../TestCases/Scss/test.min.css" };
            expected_output = "../../../TestCases/MinCss/test.min.css";
            DeleteTemporaryFiles();
        }
        [Test]
        public void CallTest() => Test();
    }
    public class AutoPipelineRealTests : TestsBase
    {
        [SetUp]
        public void CreatePipeline()
        {
            pipeline = (file) => new Compilers(new WebCompiler.Configuration.Config
            {
                Minifiers = new WebCompiler.Configuration.MinificationSettings
                {
                    Css = new WebCompiler.Configuration.Settings.CssMinifySettings
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
    public class AutoPipelineRealJustZipTests : TestsBase
    {
        [SetUp]
        public void CreatePipeline()
        {
            pipeline = (file) => new Compilers(new WebCompiler.Configuration.Config
            {
                Minifiers = new WebCompiler.Configuration.MinificationSettings
                {
                    Css = new WebCompiler.Configuration.Settings.CssMinifySettings
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
    public class AutoPipelineRealNoGzipTests : TestsBase
    {
        [SetUp]
        public void CreatePipeline()
        {
            pipeline = (file) => new Compilers(new WebCompiler.Configuration.Config
            {
                Minifiers = new WebCompiler.Configuration.MinificationSettings
                {
                    GZip = false
                }
            }, "../../../TestCases/Scss/").TryCompile(file);
            input = "../../../TestCases/Scss/site.scss";
            output_files = new List<string> { "../../../TestCases/Scss/site.css", "../../../TestCases/Scss/site.min.css" };
            expected_output = "../../../TestCases/MinCss/site.min.css";
            DeleteTemporaryFiles();
        }
        [Test]
        public void CallTest() => Test();
    }
    public class AutoPipelineRealCompileOnlyTests : TestsBase
    {
        [SetUp]
        public void CreatePipeline()
        {
            pipeline = (file) => new Compilers(new WebCompiler.Configuration.Config
            {
                Minifiers = new WebCompiler.Configuration.MinificationSettings
                {
                    Enabled = false,
                    GZip = false
                }
            }, "../../../TestCases/Scss/").TryCompile(file);
            input = "../../../TestCases/Scss/site.scss";
            output_files = new List<string> { "../../../TestCases/Scss/site.css" };
            expected_output = "../../../TestCases/Css/site.css";
            DeleteTemporaryFiles();
        }
        [Test]
        public void CallTest() => Test();
    }
}