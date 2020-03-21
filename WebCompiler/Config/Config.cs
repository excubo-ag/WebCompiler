using System.Collections.Generic;
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
        public string OutputFile { get; set; }

        /// <summary>
        /// The relative file path to the input file.
        /// </summary>
        public string InputFile { get; set; }

        /// <summary>
        /// Settings for the minification.
        /// </summary>
        public MinificationSettings Minifiers { get; set; } = new MinificationSettings();

        /// <summary>
        /// If true a source map file is generated for the file types that support it.
        /// </summary>
        public bool SourceMap { get; set; }

        /// <summary>
        /// Options specific to each compiler. Based on the inputFile property.
        /// </summary>
        public CompilerSettingsCollection Compilers { get; set; } = new CompilerSettingsCollection();

        public Dictionary<string, object> Minify { get; set; }
        public Dictionary<string, object> Options { get; set; }
        public void ApplyMinify()
        {
            if (Minify == null || !Minify.Any())
            {
                return;
            }
            Minify = Minify.ToDictionary(kv => kv.Key.ToLowerInvariant(), kv => kv.Value);
            switch (Path.GetExtension(InputFile).ToUpperInvariant())
            {
                case ".LESS":
                case ".SCSS":
                case ".SASS":
                case ".STYL":
                case ".STYLUS":
                    Minifiers.Css.ChangeSettings(Minify);
                    break;
                case ".HANDLEBARS":
                case ".HBS":
                case ".COFFEE":
                case ".ICED":
                case ".JS":
                case ".JSX":
                case ".ES6":
                    Minifiers.Javascript.ChangeSettings(Minify);
                    break;
            }

        }
        public void ApplyOptions()
        {
            if (Options == null || !Options.Any())
            {
                return;
            }
            Options = Options.ToDictionary(kv => kv.Key.ToLowerInvariant(), kv => kv.Value);
            switch (Path.GetExtension(InputFile).ToUpperInvariant())
            {
                case ".LESS":
                    Compilers.Less.ChangeSettings(Options);
                    break;
                case ".HANDLEBARS":
                case ".HBS":
                    Compilers.Handlebars.ChangeSettings(Options);
                    break;
                case ".SCSS":
                case ".SASS":
                    Compilers.Sass.ChangeSettings(Options);
                    break;
                case ".STYL":
                case ".STYLUS":
                    Compilers.Stylus.ChangeSettings(Options);
                    break;
                case ".COFFEE":
                case ".ICED":
                    Compilers.IcedCoffeeScript.ChangeSettings(Options);
                    break;
                case ".JS":
                case ".JSX":
                case ".ES6":
                    Compilers.Babel.ChangeSettings(Options);
                    break;
            }
        }
        internal string Output { get; set; }
        public bool RelativeUrls { get; set; }

        /// <summary>
        /// Converts the relative input file to an absolute file path.
        /// </summary>
        public FileInfo GetAbsoluteInputFile()
        {
            var folder = new FileInfo(FileName).DirectoryName;
            return new FileInfo(Path.Combine(folder, InputFile.Replace('/', Path.DirectorySeparatorChar)));
        }

        /// <summary>
        /// Converts the relative output file to an absolute file path.
        /// </summary>
        public FileInfo GetAbsoluteOutputFile()
        {
            var folder = new FileInfo(FileName).DirectoryName;
            return new FileInfo(Path.Combine(folder, OutputFile.Replace('/', Path.DirectorySeparatorChar)));
        }

        /// <summary>
        /// Checks to see if the input file needs compilation
        /// </summary>
        public bool CompilationRequired()
        {
            var input = GetAbsoluteInputFile();
            var output = GetAbsoluteOutputFile();

            if (!output.Exists)
            {
                return true;
            }

            if (input.LastWriteTimeUtc > output.LastWriteTimeUtc)
            {
                return true;
            }

            return HasDependenciesNewerThanOutput(input, output);
        }

        private bool HasDependenciesNewerThanOutput(FileInfo input, FileInfo output)
        {
            var projectRoot = new FileInfo(FileName).DirectoryName;
            var dependencies = DependencyService.GetDependencies(projectRoot, input.FullName);

            if (dependencies != null)
            {
                var key = input.FullName;
                return CheckForNewerDependenciesRecursively(key, dependencies, output);
            }

            return false;
        }

        private bool CheckForNewerDependenciesRecursively(string key, Dictionary<string, Dependencies> dependencies, FileInfo output, HashSet<string>? checkedDependencies = null)
        {
            if (checkedDependencies == null)
            {
                checkedDependencies = new HashSet<string>();
            }

            checkedDependencies.Add(key);

            if (!dependencies.ContainsKey(key))
            {
                return false;
            }

            foreach (var file in dependencies[key].DependentOn.ToArray())
            {
                if (checkedDependencies.Contains(file))
                {
                    continue;
                }

                var fileInfo = new FileInfo(file);

                if (!fileInfo.Exists)
                {
                    continue;
                }

                if (fileInfo.LastWriteTimeUtc > output.LastWriteTimeUtc)
                {
                    return true;
                }

                if (CheckForNewerDependenciesRecursively(file, dependencies, output, checkedDependencies))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
