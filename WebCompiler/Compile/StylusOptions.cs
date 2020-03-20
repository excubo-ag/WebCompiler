namespace WebCompiler
{
    /// <summary>
    /// Give all options for the LESS compiler
    /// </summary>
    public class StylusOptions : BaseOptions<StylusOptions>
    {
        /// <summary> Creates a new instance of the class.</summary>
        public StylusOptions()
        { }

        /// <summary>
        /// Load the settings from the config object
        /// </summary>
        protected override void LoadSettings(Config config)
        {
            base.LoadSettings(config);
        }

        /// <summary>
        /// The file name should match the compiler name
        /// </summary>
        protected override string CompilerFileName => "stylus";
    }
}
