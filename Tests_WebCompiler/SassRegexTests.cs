using NUnit.Framework;
using System.Linq;
using WebCompiler.Compile;

namespace Tests_WebCompiler
{
    public class SassRegexTests : TestsBase
    {
        [TestCase("", ExpectedResult = false)]
        [TestCase("import \"foo.scss\";", ExpectedResult = false)]
        [TestCase("@import \"foo.scss\";", ExpectedResult = true)]
        [TestCase("@import 'foo.scss';", ExpectedResult = true)]
        [TestCase("@import url(\"foo.scss\");", ExpectedResult = true)]
        [TestCase("@import url('foo.scss');", ExpectedResult = true)]
        [TestCase("@use \"foo.scss\";", ExpectedResult = true)]
        [TestCase("@use 'foo.scss';", ExpectedResult = true)]
        [TestCase("@use url(\"foo.scss\");", ExpectedResult = true)]
        [TestCase("@use url('foo.scss');", ExpectedResult = true)]
        [TestCase("@forward \"foo.scss\";", ExpectedResult = true)]
        [TestCase("@forward 'foo.scss';", ExpectedResult = true)]
        [TestCase("@forward url(\"foo.scss\");", ExpectedResult = true)]
        [TestCase("@forward url('foo.scss');", ExpectedResult = true)]
        [TestCase("@umport \"foo.scss\";", ExpectedResult = false)]
        [TestCase("@umport 'foo.scss';", ExpectedResult = false)]
        [TestCase("@umport url(\"foo.scss\");", ExpectedResult = false)]
        [TestCase("@umport url('foo.scss');", ExpectedResult = false)]
        public bool Test(string input)
        {
            var matches = SassCompiler.SassDependencyRegex.Matches(input);
            return matches.Any();
        }
    }
}