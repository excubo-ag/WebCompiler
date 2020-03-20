using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace WebCompiler
{
    /// <summary>
    /// Represents a configuration object used by the compilers.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// The file path to the configuration file.
        /// </summary>
        [JsonIgnore]
        public string FileName { get; set; }

        /// <summary>
        /// The relative file path to the output file.
        /// </summary>
        public string outputFile { get; set; }

        /// <summary>
        /// The relative file path to the input file.
        /// </summary>
        public string inputFile { get; set; }

        /// <summary>
        /// Settings for the minification.
        /// </summary>
        public Dictionary<string, object> minify { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// If true it makes Visual Studio include the output file in the project.
        /// </summary>
        [DefaultValue(true)]
        public bool includeInProject { get; set; } = true;

        /// <summary>
        /// If true a source map file is generated for the file types that support it.
        /// </summary>
        public bool sourceMap { get; set; }

        /// <summary>
        /// Options specific to each compiler. Based on the inputFile property.
        /// </summary>
        public Dictionary<string, object> options { get; set; } = new Dictionary<string, object>();

        internal string Output { get; set; }

        /// <summary>
        /// Converts the relative input file to an absolute file path.
        /// </summary>
        public FileInfo GetAbsoluteInputFile()
        {
            string folder = new FileInfo(FileName).DirectoryName;
            return new FileInfo(Path.Combine(folder, inputFile.Replace('/', Path.PathSeparator)));
        }

        /// <summary>
        /// Converts the relative output file to an absolute file path.
        /// </summary>
        public FileInfo GetAbsoluteOutputFile()
        {
            string folder = new FileInfo(FileName).DirectoryName;
            return new FileInfo(Path.Combine(folder, outputFile.Replace('/', Path.PathSeparator)));
        }

        /// <summary>
        /// Checks to see if the input file needs compilation
        /// </summary>
        public bool CompilationRequired()
        {
            FileInfo input = GetAbsoluteInputFile();
            FileInfo output = GetAbsoluteOutputFile();

            if (!output.Exists)
                return true;

            if (input.LastWriteTimeUtc > output.LastWriteTimeUtc)
                return true;

            return HasDependenciesNewerThanOutput(input, output);
        }

        private bool HasDependenciesNewerThanOutput(FileInfo input, FileInfo output)
        {
            var projectRoot = new FileInfo(FileName).DirectoryName;
            var dependencies = DependencyService.GetDependencies(projectRoot, input.FullName);

            if (dependencies != null)
            {
                string key = input.FullName.ToLowerInvariant();
                return CheckForNewerDependenciesRecursively(key, dependencies, output);
            }

            return false;
        }

        private bool CheckForNewerDependenciesRecursively(string key, Dictionary<string, Dependencies> dependencies, FileInfo output, HashSet<string> checkedDependencies = null)
        {
            if (checkedDependencies == null)
                checkedDependencies = new HashSet<string>();

            checkedDependencies.Add(key);

            if (!dependencies.ContainsKey(key))
                return false;

            foreach (var file in dependencies[key].DependentOn.ToArray())
            {
                if (checkedDependencies.Contains(file))
                    continue;

                var fileInfo = new FileInfo(file);

                if (!fileInfo.Exists)
                    continue;

                if (fileInfo.LastWriteTimeUtc > output.LastWriteTimeUtc)
                    return true;

                if (CheckForNewerDependenciesRecursively(file, dependencies, output, checkedDependencies))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if the object is equals to the other object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;
            if (obj == this) return true;

            Config other = (Config)obj;

            return GetHashCode() == other.GetHashCode();
        }

        /// <summary>
        /// Returns the hash code for this Config
        /// </summary>
        public override int GetHashCode()
        {
            return outputFile.GetHashCode();
        }

        /// <summary>For the JSON.NET serializer</summary>
        public bool ShouldSerializeIncludeInProject()
        {
            Config config = new Config();
            return includeInProject != config.includeInProject;
        }

        /// <summary>For the JSON.NET serializer</summary>
        public bool ShouldSerializeMinify()
        {
            Config config = new Config();
            return !DictionaryEqual(minify, config.minify, null);
        }

        /// <summary>For the JSON.NET serializer</summary>
        public bool ShouldSerializeOptions()
        {
            Config config = new Config();
            return !DictionaryEqual(options, config.options, null);
        }

        private static bool DictionaryEqual<TKey, TValue>(
            IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second,
            IEqualityComparer<TValue> valueComparer)
        {
            if (first == second) return true;
            if ((first == null) || (second == null)) return false;
            if (first.Count != second.Count) return false;

            valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;

            foreach (var kvp in first)
            {
                TValue secondValue;
                if (!second.TryGetValue(kvp.Key, out secondValue)) return false;
                if (!valueComparer.Equals(kvp.Value, secondValue)) return false;
            }
            return true;
        }
    }
}
