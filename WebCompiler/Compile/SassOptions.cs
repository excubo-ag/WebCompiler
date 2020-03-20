namespace WebCompiler
{
    /// <summary>
    /// Give all options for the Sass compiler
    /// </summary>
    public class SassOptions : BaseOptions<SassOptions>
    {
        private const string trueStr = "true";

        /// <summary> Creates a new instance of the class.</summary>
        public SassOptions()
        { }

        /// <summary>
        /// Loads the settings based on the config
        /// </summary>
        protected override void LoadSettings(Config config)
        {
            base.LoadSettings(config);

            var autoPrefix = GetValue(config, "autoPrefix");
            if (autoPrefix != null)
                this.autoPrefix = autoPrefix;

            if (config.options.ContainsKey("outputStyle"))
                outputStyle = config.options["outputStyle"].ToString();

            if (config.options.ContainsKey("indentType"))
                indentType = config.options["indentType"].ToString();

            if (int.TryParse(GetValue(config, "precision"), out var precision))
                this.Precision = precision;

            if (int.TryParse(GetValue(config, "indentWidth"), out var indentWidth))
                this.indentWidth = indentWidth;

            var relativeUrls = GetValue(config, "relativeUrls");
            if (relativeUrls != null)
                this.relativeUrls = relativeUrls.ToLowerInvariant() == trueStr;

            var includePath = GetValue(config, "includePath");
            if (includePath != null)
                this.includePath = includePath;

            var sourceMapRoot = GetValue(config, "sourceMapRoot");
            if (sourceMapRoot != null)
                this.sourceMapRoot = sourceMapRoot;

            var lineFeed = GetValue(config, "lineFeed");
            if (lineFeed != null)
                this.lineFeed = lineFeed;
        }

        /// <summary>
        /// The file name should match the compiler name
        /// </summary>
        protected override string CompilerFileName
        {
            get { return "sass"; }
        }

        /// <summary>
        /// Autoprefixer will use the data based on current browser popularity and
        /// property support to apply prefixes for you.
        /// </summary>
        public string autoPrefix { get; set; } = "";

        /// <summary>
        /// Path to look for imported files
        /// </summary>
        public string includePath { get; set; } = string.Empty;

        /// <summary>
        /// Indent type for output CSS.
        /// </summary>
        public string indentType { get; set; } = "space";

        /// <summary>
        /// Number of spaces or tabs (maximum value: 10)
        /// </summary>
        public int indentWidth { get; set; } = 2;

        /// <summary>
        /// Type of output style
        /// </summary>
        public string outputStyle { get; set; } = "nested";


        /// <summary>
        /// Precision
        /// </summary>
        public int Precision { get; set; } = 5;

        /// <summary>
        /// This option allows you to re-write URL's in imported files so that the URL is always
        /// relative to the base imported file.
        /// </summary>
        public bool relativeUrls { get; set; } = true;

        /// <summary>
        /// Base path, will be emitted in source-map as is
        /// </summary>
        public string sourceMapRoot { get; set; } = string.Empty;

        /// <summary>
        /// Linefeed style (cr | crlf | lf | lfcr)
        /// </summary>
        public string lineFeed { get; set; } = string.Empty;


    }
}
