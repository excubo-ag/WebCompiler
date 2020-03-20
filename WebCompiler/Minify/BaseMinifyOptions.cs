using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace WebCompiler
{
    public class MinifyOptionsJson
    {
        public Dictionary<string, Dictionary<string, object>> minifiers { get; set; }
    }
    /// <summary>
    /// Base class for minification options
    /// </summary>
    public abstract class BaseMinifyOptions
    {
        /// <summary>
        /// Loads the options based on the config object
        /// </summary>
        protected static void LoadDefaultSettings(Config config, string minifierType)
        {
            string defaultFile = config.FileName + ".defaults";

            if (!File.Exists(defaultFile))
                return;


            var min = JsonSerializer.Deserialize<MinifyOptionsJson>(File.ReadAllText(defaultFile));
            if (!min.minifiers.ContainsKey(minifierType))
            {
                return;
            }
            var options = min.minifiers[minifierType];

            if (options != null)
            {
                foreach (string key in options.Keys)
                {
                    if (!config.minify.ContainsKey(key))
                        config.minify[key] = options[key];
                }
            }
        }

        /// <summary>
        /// Gets the string value of the minification settings.
        /// </summary>
        protected static string GetValue(Config config, string key, object defaultValue = null)
        {
            if (config.minify.ContainsKey(key))
                return config.minify[key].ToString();

            if (defaultValue != null)
                return defaultValue.ToString();

            return string.Empty;
        }
    }
}
