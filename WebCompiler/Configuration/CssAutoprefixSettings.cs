using System.Collections.Generic;
using AutoprefixerHost;

namespace WebCompiler.Configuration
{
    public class CssAutoprefixSettings
    {
        public bool              Enabled           { get; set; } = true;
        public ProcessingOptions ProcessingOptions { get; set; } = new ProcessingOptions
        {
            Browsers = new List<string>{ "last 4 versions" },
            SourceMap = true
        };
    }
}