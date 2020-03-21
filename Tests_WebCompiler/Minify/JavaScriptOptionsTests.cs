using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUglify;
using NUglify.JavaScript;
using System.IO;
using System.Linq;
using WebCompiler;

namespace WebCompilerTest.Minify
{
    [TestClass]
    public class JavaScriptOptionsTests
    {
        private const string processingConfigFile = "../../../Minify/artifacts/javascript/";

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void EvalTreatmentInUpperCaseShouldWork()
        {
            var configFile = Path.Combine(processingConfigFile, "evaltreatmentmakeallsafeuppercase.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = configs.First().Minifiers.Javascript.ToCodeSettings();
            Assert.AreEqual(EvalTreatment.MakeAllSafe, cfg.EvalTreatment);
        }

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void EvalTreatmentIgnore()
        {
            var configFile = Path.Combine(processingConfigFile, "evaltreatmentignore.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = configs.First().Minifiers.Javascript.ToCodeSettings();
            Assert.AreEqual(EvalTreatment.Ignore, cfg.EvalTreatment);
        }

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void EvalTreatmentMakeAllSafe()
        {
            var configFile = Path.Combine(processingConfigFile, "evaltreatmentmakeallsafe.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = configs.First().Minifiers.Javascript.ToCodeSettings();
            Assert.AreEqual(EvalTreatment.MakeAllSafe, cfg.EvalTreatment);
        }

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void EvalTreatmentMakeImmediateSafee()
        {
            var configFile = Path.Combine(processingConfigFile, "evaltreatmentmakeimmediatesafe.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = configs.First().Minifiers.Javascript.ToCodeSettings();
            Assert.AreEqual(EvalTreatment.MakeImmediateSafe, cfg.EvalTreatment);
        }

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void OutputModeInUpperCaseShouldWork()
        {
            var configFile = Path.Combine(processingConfigFile, "outputmodemultiplelinesuppercase.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = configs.First().Minifiers.Javascript.ToCodeSettings();
            Assert.AreEqual(OutputMode.MultipleLines, cfg.OutputMode);
        }

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void OutputModeAllInLowerCaseShouldWork()
        {
            var configFile = Path.Combine(processingConfigFile, "outputmodemultiplelineslowercase.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = configs.First().Minifiers.Javascript.ToCodeSettings();
            Assert.AreEqual(OutputMode.MultipleLines, cfg.OutputMode);
        }

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void OutputModeMultipleLines()
        {
            var configFile = Path.Combine(processingConfigFile, "outputmodemultiplelines.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = configs.First().Minifiers.Javascript.ToCodeSettings();
            Assert.AreEqual(OutputMode.MultipleLines, cfg.OutputMode);
        }

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void OutputModeNone()
        {
            var configFile = Path.Combine(processingConfigFile, "outputmodenone.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = configs.First().Minifiers.Javascript.ToCodeSettings();
            Assert.AreEqual(OutputMode.None, cfg.OutputMode);
        }

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void OutputModeSingleLine()
        {
            var configFile = Path.Combine(processingConfigFile, "outputmodesingleline.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = configs.First().Minifiers.Javascript.ToCodeSettings();
            Assert.AreEqual(OutputMode.SingleLine, cfg.OutputMode);
        }

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void IndentSize()
        {
            var configFile = Path.Combine(processingConfigFile, "indentsize.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = configs.First().Minifiers.Javascript.ToCodeSettings();
            Assert.AreEqual(8, cfg.IndentSize);
        }
    }
}
