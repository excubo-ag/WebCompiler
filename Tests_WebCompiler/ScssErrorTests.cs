using NUnit.Framework;
using System.Linq;
using WebCompiler.Compile;
using WebCompiler.Configuration.Settings;

namespace Tests_WebCompiler
{
    public class ScssErrorTests
    {
        [Test]
        public void Test()
        {
            var compiler = new SassCompiler(new SassSettings());
            var result = compiler.Compile("../../../TestCases/Scss/error.scss");
            Assert.IsTrue(result.Errors != null && result.Errors.Any());
        }
    }
}