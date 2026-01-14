using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebCompiler.Compile;
using WebCompiler.Configuration;
using WebCompiler.Configuration.Settings;

namespace Tests_WebCompiler
{
    public class SassOmitSourceMapUrlTests : TestsBase
    {
        [TestCase(true)]
        [TestCase(false)]
        public void TestOmitSourceMapUrl(bool omitSourceMapUrl)
        {
            // Setup pipeline with OmitSourceMapUrl parameter
            pipeline = (file) =>
                    new CompilationStep(file)
                           .With(new SassCompiler(new SassSettings { SourceMap = true, OmitSourceMapUrl = omitSourceMapUrl }));
            input = "../../../TestCases/Scss/test.scss";
            output_files = new List<string> { "../../../TestCases/Scss/test.css" };
            
            DeleteOutputFiles();
            
            // Process the file
            CompilationStep result = null;
            Assert.DoesNotThrow(() => result = pipeline(input), "Compiling should not result in exception");
            Assert.That(result, Is.Not.Null, "Compilation result may not be null");
            Assert.That(result.Errors == null || !result.Errors.Any(), Is.True, "Compilation should not result in error");
            
            // Verify the CSS file was created
            var cssFile = "../../../TestCases/Scss/test.css";
            Assert.That(File.Exists(cssFile), Is.True, "CSS file should be created");
            
            // Read the CSS content
            var cssContent = File.ReadAllText(cssFile);
            
            // Verify sourceMappingURL presence based on the parameter
            if (omitSourceMapUrl)
            {
                Assert.That(cssContent.Contains("sourceMappingURL"), Is.False, 
                    "CSS file should not contain sourceMappingURL comment when OmitSourceMapUrl is true");
            }
            else
            {
                Assert.That(cssContent.Contains("sourceMappingURL"), Is.True, 
                    "CSS file should contain sourceMappingURL comment when OmitSourceMapUrl is false");
            }
        }
    }
}
