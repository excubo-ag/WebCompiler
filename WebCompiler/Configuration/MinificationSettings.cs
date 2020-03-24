using WebCompiler.Configuration.Settings;

namespace WebCompiler.Configuration
{
    public class MinificationSettings
    {
        public bool GZip { get; set; } = true;
        public bool Enabled { get; set; } = true;
        public CssMinifySettings Css { get; set; } = new CssMinifySettings();
        public JavaScriptMinifySettings Javascript { get; set; } = new JavaScriptMinifySettings();
    }
}