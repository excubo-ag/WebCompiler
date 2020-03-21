using System.Collections.Generic;

namespace WebCompiler
{
    public class BaseSettings
    {
        public bool SourceMap { get; set; }
        public virtual void ChangeSettings(Dictionary<string, object> values)
        {
            if (values.ContainsKey(nameof(SourceMap).ToLowerInvariant()))
            {
                SourceMap = bool.Parse(values[nameof(SourceMap).ToLowerInvariant()].ToString());
            }
        }
    }
}
