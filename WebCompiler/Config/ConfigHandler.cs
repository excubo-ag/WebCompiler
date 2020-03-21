using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace WebCompiler
{
    /// <summary>
    /// Handles reading and writing config files to disk.
    /// </summary>
    public class ConfigHandler
    {
        /// <summary>
        /// Adds a config file if no one exist or adds the specified config to an existing config file.
        /// </summary>
        /// <param name="fileName">The file path of the configuration file.</param>
        /// <param name="config">The compiler config object to add to the configration file.</param>
        public void AddConfig(string fileName, Config config)
        {
            var existing = GetConfigs(fileName);
            var configs = new List<Config>();
            configs.AddRange(existing);
            configs.Add(config);
            config.FileName = fileName;

            var settings = new JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                WriteIndented = true
            };

            var content = JsonSerializer.Serialize(configs, settings);
            File.WriteAllText(fileName, content, new UTF8Encoding(true));
        }

        /// <summary>
        /// Removes the specified config from the file.
        /// </summary>
        public void RemoveConfig(Config configToRemove)
        {
            var configs = GetConfigs(configToRemove.FileName);
            var newConfigs = new List<Config>();

            if (configs.Contains(configToRemove))
            {
                newConfigs.AddRange(configs.Where(b => !b.Equals(configToRemove)));
                var content = JsonSerializer.Serialize(newConfigs, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configToRemove.FileName, content);
            }
        }

        /// <summary>
        /// Creates a file containing the default compiler options if one doesn't exist.
        /// </summary>
        public void CreateDefaultsFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                return;
            }

            var defaults = new
            {
                compilers = new
                {
                    less = new LessSettings(),
                    sass = new SassSettings(),
                    stylus = new StylusSettings(),
                    babel = new BabelSettings(),
                    coffeescript = new IcedCoffeeScriptSettings(),
                    handlebars = new HandlebarsSettings(),
                },
                minifiers = new
                {
                    css = new
                    {
                        enabled = true,
                        termSemicolons = true,
                        gzip = false
                    },
                    javascript = new
                    {
                        enabled = true,
                        termSemicolons = true,
                        gzip = false
                    },
                }
            };

            var json = JsonSerializer.Serialize(defaults, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fileName, json);
        }

        /// <summary>
        /// Get all the config objects in the specified file.
        /// </summary>
        /// <param name="fileName">A relative or absolute file path to the configuration file.</param>
        /// <returns>A list of Config objects.</returns>
        public static IEnumerable<Config> GetConfigs(string fileName)
        {
            var file = new FileInfo(fileName);

            if (!file.Exists)
            {
                return Enumerable.Empty<Config>();
            }

            var content = File.ReadAllText(fileName);
            var options = new JsonSerializerOptions { AllowTrailingCommas = true, PropertyNameCaseInsensitive = true, IgnoreNullValues = true, IgnoreReadOnlyProperties = true };
            options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            var configs = JsonSerializer.Deserialize<List<Config>>(content, options);

            _ = Path.GetDirectoryName(file.FullName);

            var defaults = File.Exists(fileName + ".defaults") ? JsonSerializer.Deserialize<Config>(File.ReadAllText(fileName + ".defaults"), options) : default;

            foreach (var config in configs)
            {
                if (defaults != null)
                {
                    config.Compilers = defaults.Compilers;
                    config.Minifiers = defaults.Minifiers;
                }
                config.ApplyMinify();
                config.ApplyOptions();
                config.FileName = fileName;
            }

            return configs;
        }
    }
}
