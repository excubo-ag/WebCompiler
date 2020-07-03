using NUnit.Framework;
using System.Collections.Generic;
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
            var result = compiler.Compile(new List<(string File, bool Created)> { (File: "../../../TestCases/Scss/error.scss", Created: false) });
            Assert.IsTrue(result.Errors != null && result.Errors.Any());
        }
    }
}