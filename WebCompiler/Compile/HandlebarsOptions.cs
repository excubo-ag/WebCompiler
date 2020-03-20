using System.Linq;

namespace WebCompiler
{
    /// <summary>
    /// Give all options for the Handlebars compiler
    /// </summary>
    public class HandlebarsOptions : BaseOptions<HandlebarsOptions>
    {
        private const string trueStr = "true";

        /// <summary> Creates a new instance of the class.</summary>
        public HandlebarsOptions()
        { }

        /// <summary>
        /// Load the settings from the config object
        /// </summary>
        protected override void LoadSettings(Config config)
        {
            base.LoadSettings(config);

            var name = GetValue(config, "name");
            if (name != null)
                this.name = name;

            var @namespace = GetValue(config, "namespace");
            if (@namespace != null)
                this.@namespace = @namespace;

            var root = GetValue(config, "root");
            if (root != null)
                this.root = root;

            var commonjs = GetValue(config, "commonjs");
            if (commonjs != null)
                this.commonjs = commonjs;

            var amd = GetValue(config, "amd");
            if (amd != null)
                this.amd = amd.ToLowerInvariant() == trueStr;

            var forcePartial = GetValue(config, "forcePartial");
            if (forcePartial != null)
                this.forcePartial = forcePartial.ToLowerInvariant() == trueStr;

            var noBOM = GetValue(config, "noBOM");
            if (noBOM != null)
                this.noBOM = noBOM.ToLowerInvariant() == trueStr;

            var knownHelpersOnly = GetValue(config, "knownHelpersOnly");
            if (knownHelpersOnly != null)
                this.knownHelpersOnly = knownHelpersOnly.ToLowerInvariant() == trueStr;

            var knownHelpers = GetValue(config, "knownHelpers");
            if (knownHelpers != null)            
                this.knownHelpers = knownHelpers.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
        }

        /// <summary>
        /// The file name should match the compiler name
        /// </summary>
        protected override string CompilerFileName
        {
            get { return "hbs"; }
        }

        /// <summary>
        /// Template root. Base value that will be stripped from template names.
        /// </summary>
        public string root { get; set; } = "";

        /// <summary>
        /// Removes the BOM (Byte Order Mark) from the beginning of the templates.
        /// </summary>
        public bool noBOM { get; set; } = false;

        /// <summary>
        /// Name of passed string templates.
        /// </summary>
        public string name { get; set; } = "";

        /// <summary>
        /// Template namespace 
        /// </summary>
        public string @namespace { get; set; } = "";

        /// <summary>
        /// Compile with known helpers only
        /// </summary>
        public bool knownHelpersOnly { get; set; } = false;


        /// <summary>
        /// Forcing a partial template compilation
        /// </summary>
        public bool forcePartial { get; set; } = false;

        /// <summary>
        /// List of known helpers for a more optimized output
        /// </summary>
        public string[] knownHelpers { get; set; } = new string[0];

        /// <summary>
        /// Path to the Handlebars module to export CommonJS style
        /// </summary>
        public string commonjs { get; set; } = "";

        /// <summary>
        /// Exports amd style (require.js), this option has priority to commonjs.
        /// </summary>
        public bool amd { get; set; } = false;
    }
}
