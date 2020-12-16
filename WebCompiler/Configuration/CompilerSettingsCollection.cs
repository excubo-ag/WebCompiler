using WebCompiler.Configuration.Settings;

namespace WebCompiler.Configuration
{
    public class CompilerSettingsCollection
    {
        public SassSettings Sass { get; set; } = new SassSettings();
    }
}