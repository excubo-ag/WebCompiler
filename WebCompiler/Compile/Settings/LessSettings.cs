using System.Collections.Generic;

namespace WebCompiler
{
    /// <summary>
    /// Give all options for the LESS compiler
    /// </summary>
    public class LessSettings : BaseSettings
    {
        /// <summary>
        /// CssComb will order the properties in the compiled CSS file.
        /// </summary>
        public string CssComb { get; set; } = "none";
        /// <summary>
        /// Currently only used for the data-uri function to ensure that images aren't
        /// created that are too large for the browser to handle.
        /// </summary>
        public bool IeCompat { get; set; } = true;
        /// <summary>
        /// New option for math that replaces 'strictMath' option.
        /// </summary>
        public string? Math { get; set; } = null;
        /// <summary>
        /// Without this option on Less will try and process all maths in your CSS.
        /// </summary>
        public bool StrictMath { get; set; } = false;
        /// <summary>
        /// Without this option, less attempts to guess at the output unit when it does maths.
        /// </summary>
        public bool StrictUnits { get; set; } = false;
        /// <summary>
        /// This option allows you to re-write URL's in imported files so that the URL is always
        /// relative to the base imported file.
        /// </summary>
        public bool RelativeUrls { get; set; } = true;
        /// <summary>
        /// Allows you to add a path to every generated import and url in your css.
        /// This does not affect less import statements that are processed,
        /// just ones that are left in the output css.
        /// </summary>
        public string RootPath { get; set; }
        /// <summary>
        /// Base path, will be emitted in source-map as is
        /// </summary>
        public string SourceMapRoot { get; set; }
        /// <summary>
        /// This is the opposite of the 'rootpath' option, it specifies a path which should be removed from the output paths.
        /// </summary>
        public string SourceMapBasePath { get; set; }
        public override void ChangeSettings(Dictionary<string, object> values)
        {
            if (values.ContainsKey(nameof(CssComb).ToLowerInvariant()))
            {
                CssComb = values[nameof(CssComb).ToLowerInvariant()].ToString();
            }
            if (values.ContainsKey(nameof(IeCompat).ToLowerInvariant()))
            {
                IeCompat = bool.Parse(values[nameof(IeCompat).ToLowerInvariant()].ToString());
            }
            if (values.ContainsKey(nameof(Math).ToLowerInvariant()))
            {
                Math = values[nameof(Math).ToLowerInvariant()].ToString();
            }
            if (values.ContainsKey(nameof(StrictMath).ToLowerInvariant()))
            {
                StrictMath = bool.Parse(values[nameof(StrictMath).ToLowerInvariant()].ToString());
            }
            if (values.ContainsKey(nameof(StrictUnits).ToLowerInvariant()))
            {
                StrictUnits = bool.Parse(values[nameof(StrictUnits).ToLowerInvariant()].ToString());
            }
            if (values.ContainsKey(nameof(RelativeUrls).ToLowerInvariant()))
            {
                RelativeUrls = bool.Parse(values[nameof(RelativeUrls).ToLowerInvariant()].ToString());
            }
            if (values.ContainsKey(nameof(RootPath).ToLowerInvariant()))
            {
                RootPath = values[nameof(RootPath).ToLowerInvariant()].ToString();
            }
            if (values.ContainsKey(nameof(SourceMapRoot).ToLowerInvariant()))
            {
                SourceMapRoot = values[nameof(SourceMapRoot).ToLowerInvariant()].ToString();
            }
            if (values.ContainsKey(nameof(SourceMapBasePath).ToLowerInvariant()))
            {
                SourceMapBasePath = values[nameof(SourceMapBasePath).ToLowerInvariant()].ToString();
            }
            base.ChangeSettings(values);
        }
    }
}
