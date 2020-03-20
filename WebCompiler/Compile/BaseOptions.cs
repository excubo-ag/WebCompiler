using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace WebCompiler
{
    public class CompilerDefaults
    {
        public Dictionary<string, object> compilers { get; set; }
    }
    /// <summary>
    /// Base class containing methods to all extensions options
    /// </summary>
    public abstract class BaseOptions<T> where T : BaseOptions<T>, new()
    {
        /// <summary>
        /// Loads the options based on the config object
        /// </summary>
        public static T FromConfig(Config config)
        {
            string defaultFile = config.FileName + ".defaults";

            T options = new T();

            if (File.Exists(defaultFile))
            {
                CompilerDefaults json = JsonSerializer.Deserialize<CompilerDefaults>(File.ReadAllText(defaultFile));
                object jsonOptions = json.compilers.ContainsKey(options.CompilerFileName) ? json.compilers[options.CompilerFileName] : default;

                if (jsonOptions != null)
                {
                    options = JsonSerializer.Deserialize<T>(jsonOptions.ToString());
                }
            }

            options.LoadSettings(config);

            return options;
        }

        /// <summary>
        /// The file name should match the compiler name
        /// </summary>
        protected abstract string CompilerFileName { get; }

        /// <summary>
        /// Load the settings from the config object
        /// </summary>
        protected virtual void LoadSettings(Config config)
        {
            string sourceMap = GetValue(config, "sourceMap");
            if (sourceMap != null)
            {
                this.sourceMap = sourceMap.ToLowerInvariant() == "true";
            }
        }

        /// <summary>
        /// Generate Source Map v3
        /// </summary>
        public bool sourceMap { get; set; }

        /// <summary>
        /// Gets a value from a string keyed dictionary
        /// </summary>
        protected string GetValue(Config config, string key)
        {
            if (config.options.ContainsKey(key))
            {
                return config.options[key].ToString();
            }

            return null;
        }
    }
}
