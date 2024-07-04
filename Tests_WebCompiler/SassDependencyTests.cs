using NUnit.Framework;
using System.IO;
using System.Linq;
using WebCompiler.Compile;
using WebCompiler.Configuration.Settings;

namespace Tests_WebCompiler
{
    internal class SassDependencyTests
    {
        [Test]
        public void TestUseWithOverrides()
        {
            var compiler = new SassCompiler(new SassSettings());
            var dependencies = compiler.GetDependencies("../../../TestCases/Scss/use_with_override.scss");
            Assert.That(dependencies.Select(Path.GetFileName).OrderBy(v => v), Is.EqualTo(new[] {
                "use_with_override.scss", 
                "dependency.scss", 
                "test.scss", 
                "_variables.scss", 
                "relative.scss", 
                "foo.scss", 
                "_bar.scss" }.OrderBy(v => v)));
        }
    }
}
