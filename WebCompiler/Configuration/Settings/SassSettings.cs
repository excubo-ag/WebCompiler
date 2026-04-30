using System;

using DartSassHost;

namespace WebCompiler.Configuration.Settings
{
    public class SassSettings : BaseCompileSettings
    {
        public string? IncludePath { get; set; }
        [Obsolete]
        public IndentType IndentType { get; set; } = IndentType.Space;
        [Obsolete]
        public int IndentWidth { get; set; } = 2;
        public OutputStyle OutputStyle { get; set; } = OutputStyle.Expanded;
        public bool RelativeUrls { get; set; } = true;
        public string? SourceMapRoot { get; set; }
        [Obsolete]
        public LineFeedType LineFeed { get; set; } = LineFeedType.Lf;
        public bool OmitSourceMapUrl { get; set; } = false;
    }
}