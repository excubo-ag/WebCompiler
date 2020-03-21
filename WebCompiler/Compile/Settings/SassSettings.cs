using System.Collections.Generic;

namespace WebCompiler
{
    public class SassSettings : BaseSettings
    {
        public string IncludePath { get; set; }
        public string IndentType { get; set; } = "space";
        public int IndentWidth { get; set; } = 2;
        public string OutputStyle { get; set; } = "nested";
        public int Precision { get; set; } = 5;
        public bool RelativeUrls { get; set; } = true;
        public string SourceMapRoot { get; set; }
        public string LineFeed { get; set; } = "lf";
        public bool SourceMap { get; set; }
        public override void ChangeSettings(Dictionary<string, object> values)
        {
            if (values.ContainsKey(nameof(IncludePath).ToLowerInvariant()))
            {
                IncludePath = values[nameof(IncludePath).ToLowerInvariant()].ToString();
            }
            if (values.ContainsKey(nameof(IndentType).ToLowerInvariant()))
            {
                IndentType = values[nameof(IndentType).ToLowerInvariant()].ToString();
            }
            if (values.ContainsKey(nameof(IndentWidth).ToLowerInvariant()))
            {
                IndentWidth = int.Parse(values[nameof(IndentWidth).ToLowerInvariant()].ToString());
            }
            if (values.ContainsKey(nameof(OutputStyle).ToLowerInvariant()))
            {
                OutputStyle = values[nameof(OutputStyle).ToLowerInvariant()].ToString();
            }
            if (values.ContainsKey(nameof(Precision).ToLowerInvariant()))
            {
                Precision = int.Parse(values[nameof(Precision).ToLowerInvariant()].ToString());
            }
            if (values.ContainsKey(nameof(RelativeUrls).ToLowerInvariant()))
            {
                RelativeUrls = bool.Parse(values[nameof(RelativeUrls).ToLowerInvariant()].ToString());
            }
            if (values.ContainsKey(nameof(SourceMapRoot).ToLowerInvariant()))
            {
                SourceMapRoot = values[nameof(SourceMapRoot).ToLowerInvariant()].ToString();
            }
            if (values.ContainsKey(nameof(LineFeed).ToLowerInvariant()))
            {
                LineFeed = values[nameof(LineFeed).ToLowerInvariant()].ToString();
            }
            if (values.ContainsKey(nameof(SourceMap).ToLowerInvariant()))
            {
                SourceMap = bool.Parse(values[nameof(SourceMap).ToLowerInvariant()].ToString());
            }
            base.ChangeSettings(values);
        }
    }
}
