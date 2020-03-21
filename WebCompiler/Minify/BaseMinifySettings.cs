using System.Collections.Generic;

namespace WebCompiler
{
    /// <summary>
    /// Base class for minification options
    /// </summary>
    public abstract class BaseMinifySettings
    {
        public bool TermSemicolons { get; set; } = true;
        public string OutputMode { get; set; } = "singleline";
        public int IndentSize { get; set; } = 2;
        public virtual void ChangeSettings(Dictionary<string, object> values)
        {
            if (values.ContainsKey(nameof(TermSemicolons).ToLowerInvariant()))
            {
                TermSemicolons = bool.Parse(values[nameof(TermSemicolons).ToLowerInvariant()].ToString());
            }
            if (values.ContainsKey(nameof(OutputMode).ToLowerInvariant()))
            {
                OutputMode = values[nameof(OutputMode).ToLowerInvariant()].ToString();
            }
            if (values.ContainsKey(nameof(IndentSize).ToLowerInvariant()))
            {
                IndentSize = int.Parse(values[nameof(IndentSize).ToLowerInvariant()].ToString());
            }
        }
    }
}
