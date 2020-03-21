using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using WebCompiler;

namespace WebCompilerTest
{
    [TestClass]
    public class ScssOptionsTest
    {
        [TestMethod, TestCategory("SCSSOptions")]
        public void OutputStyleNested()
        {
            var configs = ConfigHandler.GetConfigs("../../../artifacts/options/scss/scssconfignested.json");
            var result = configs.First().Compilers.Sass;
            Assert.AreEqual("nested", result.OutputStyle);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void OutputStyleExpanded()
        {
            var configs = ConfigHandler.GetConfigs("../../../artifacts/options/scss/scssconfigexpanded.json");
            var result = configs.First().Compilers.Sass;
            Assert.AreEqual("expanded", result.OutputStyle);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void OutputStyleCompact()
        {
            var configs = ConfigHandler.GetConfigs("../../../artifacts/options/scss/scssconfigcompact.json");
            var result = configs.First().Compilers.Sass;
            Assert.AreEqual("compact", result.OutputStyle);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void OutputStyleCompressed()
        {
            var configs = ConfigHandler.GetConfigs("../../../artifacts/options/scss/scssconfigcompressed.json");
            var result = configs.First().Compilers.Sass;
            Assert.AreEqual("compressed", result.OutputStyle);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void Precision()
        {
            var configs = ConfigHandler.GetConfigs("../../../artifacts/options/scss/scssconfigprecision.json");
            var result = configs.First().Compilers.Sass;
            Assert.AreEqual(3, result.Precision);
        }
    }
}
