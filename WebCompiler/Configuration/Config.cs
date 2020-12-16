namespace WebCompiler.Configuration
{
    /// <summary>
    /// Represents a configuration object used by the compilers.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Settings for the minification.
        /// </summary>
        public MinificationSettings Minifiers { get; set; } = new MinificationSettings();

        public CssAutoprefixSettings Autoprefix { get; set; } = new CssAutoprefixSettings();

        /// <summary>
        /// Options specific to each compiler. Based on the inputFile property.
        /// </summary>
        public CompilerSettingsCollection CompilerSettings { get; set; } = new CompilerSettingsCollection();
        public OutputSettings Output { get; set; } = new OutputSettings();
    }
    public class OutputSettings
    {
        public string? Directory { get; set; }
        public bool Preserve { get; set; } = true;
    }
}