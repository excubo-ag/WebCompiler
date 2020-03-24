using NUglify;

namespace WebCompiler.Configuration.Settings
{
    /// <summary>
    /// Base class for minification options
    /// </summary>
    public abstract class BaseMinifySettings
    {
        public bool TermSemicolons { get; set; } = true;
        public OutputMode OutputMode { get; set; } = OutputMode.SingleLine;
        public int IndentSize { get; set; } = 2;
    }
}
