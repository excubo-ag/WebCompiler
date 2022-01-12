using WebCompiler.Configuration.Settings;
using System.Collections.Generic;

namespace WebCompiler.Configuration
{
    public class CompilerSettingsCollection
    {
        public SassSettings Sass { get; set; } = new SassSettings();
        public List<string> Ignore { get; set; } = new List<string>();
    }
}