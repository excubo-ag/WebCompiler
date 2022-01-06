using WebCompiler.Configuration.Settings;
using System.Collections.Generic;

namespace WebCompiler.Configuration
{
    public class CompilerSettingsCollection
    {
        public SassSettings Sass { get; set; } = new SassSettings();
        public List<string> IgnoreFolders { get; set; } = new List<string>();
        public List<string> IgnoreFiles { get; set; } = new List<string>();
    }
}