namespace WebCompiler
{
    /// <summary>
    /// Give all options for the LESS compiler
    /// </summary>
    public class LessOptions : BaseOptions<LessOptions>
    {
        private const string trueStr = "true";

        /// <summary> Creates a new instance of the class.</summary>
        public LessOptions()
        { }

        /// <summary>
        /// Load the settings from the config object
        /// </summary>
        protected override void LoadSettings(Config config)
        {
            base.LoadSettings(config);

            var autoPrefix = GetValue(config, "autoPrefix");
            if (autoPrefix != null)
                this.autoPrefix = autoPrefix;

            var cssComb = GetValue(config, "cssComb");
            if (cssComb != null)
                this.cssComb = cssComb;

            var ieCompat = GetValue(config, "ieCompat");
            if (ieCompat != null)
                this.ieCompat = ieCompat.ToLowerInvariant() == trueStr;

            var math = GetValue(config, "math");
            if (math != null)
                this.math = math;

            var strictMath = GetValue(config, "strictMath");
            if (strictMath != null)
                this.strictMath = strictMath.ToLowerInvariant() == trueStr;

            var strictUnits = GetValue(config, "strictUnits");
            if (strictUnits != null)
                this.strictUnits = strictUnits.ToLowerInvariant() == trueStr;

            var rootPath = GetValue(config, "rootPath");
            if (rootPath != null)
                this.rootPath = rootPath;

            var relativeUrls = GetValue(config, "relativeUrls");
            if (relativeUrls != null)
                this.relativeUrls = relativeUrls.ToLowerInvariant() == trueStr;

            var sourceMapRoot = GetValue(config, "sourceMapRoot");
            if (sourceMapRoot != null)
                this.sourceMapRoot = sourceMapRoot;

            var sourceMapBasePath = GetValue(config, "sourceMapBasePath");
            if (sourceMapBasePath != null)
                this.sourceMapBasePath = sourceMapBasePath;
        }

        /// <summary>
        /// The file name should match the compiler name
        /// </summary>
        protected override string CompilerFileName
        {
            get { return "less"; }
        }

        /// <summary>
        /// Autoprefixer will use the data based on current browser popularity and
        /// property support to apply prefixes for you.
        /// </summary>
        public string autoPrefix { get; set; } = "";

        /// <summary>
        /// CssComb will order the properties in the compiled CSS file.
        /// </summary>
        public string cssComb { get; set; } = "none";

        /// <summary>
        /// Currently only used for the data-uri function to ensure that images aren't
        /// created that are too large for the browser to handle.
        /// </summary>
        public bool ieCompat { get; set; } = true;

        /// <summary>
        /// New option for math that replaces 'strictMath' option.
        /// </summary>
        public string math { get; set; } = null;

        /// <summary>
        /// Without this option on Less will try and process all maths in your CSS.
        /// </summary>
        public bool strictMath { get; set; } = false;

        /// <summary>
        /// Without this option, less attempts to guess at the output unit when it does maths.
        /// </summary>
        public bool strictUnits { get; set; } = false;

        /// <summary>
        /// This option allows you to re-write URL's in imported files so that the URL is always
        /// relative to the base imported file.
        /// </summary>
        public bool relativeUrls { get; set; } = true;

        /// <summary>
        /// Allows you to add a path to every generated import and url in your css.
        /// This does not affect less import statements that are processed,
        /// just ones that are left in the output css.
        /// </summary>
        public string rootPath { get; set; } = "";

        /// <summary>
        /// Base path, will be emitted in source-map as is
        /// </summary>
        public string sourceMapRoot { get; set; } = string.Empty;

        /// <summary>
        /// This is the opposite of the 'rootpath' option, it specifies a path which should be removed from the output paths.
        /// </summary>
        public string sourceMapBasePath { get; set; } = string.Empty;
    }
}
