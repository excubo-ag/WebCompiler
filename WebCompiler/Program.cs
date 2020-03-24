using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebCompiler.Compile;
using WebCompiler.Configuration;

namespace WebCompiler
{
    public static class Extension
    {
        public static int IndexOfAny<T>(this IEnumerable<T> enumerable, params T[] values)
        {
            if (!values.Any())
            {
                return -1;
            }
            foreach (var (element, index) in enumerable.Select((e, i) => (e, i)))
            {
                if (values.Contains(element))
                {
                    return index;
                }
            }
            return -1;
        }
    }
    internal static class Program
    {
        private static readonly JsonSerializerOptions json_serializer_options = CreateDefaultOptions();
        private static JsonSerializerOptions CreateDefaultOptions()
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                IgnoreNullValues = true,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }

        private static void ShowHelp(bool show_file_help = false, bool show_config_help = false)
        {
            Console.WriteLine(
@"
Excubo.WebCompiler

Usage:
  webcompiler file1 file2 ... [options]
  webcompiler [options]

Options:
  -c|--config <conf.json>        Specify a configuration file for compilation.
  -d|--defaults [conf.json]      Write a default configuration file (file name is webcompilerconfiguration.json, if none is specified).
  -f|--files <files.conf>        Specify a list of files that should be compiled.
  -h|--help                      Show command line help.
  -m|--minify [disable/enable]   Enable/disable minification (default: enabled), ignored if configuration file is provided.
  -r|--recursive                 Recursively search folders for compilable files (only if any of the provided arguments is a folder).
  -z|--zip [disable/enable]      Enable/disable gzip (default: enabled), ignored if configuration file is provided.
".Trim()
            );
            if (show_file_help)
            {
                Console.WriteLine();
                Console.WriteLine(
@"
File format to specify list of files (-f|--files): one file per line
  path/to/file.scss
  path/to/other/file.scss
".Trim()
                );
            }
            if (show_config_help)
            {
                Console.WriteLine();
                Console.WriteLine(
@"
File format to specify compiler configuration (-c|--config):
    {
        ""compilers"": {
            ""sass"": {
                ""includePath"": """",
                ""indentType"": ""space"",
                ""indentWidth"": 2,
                ""outputStyle"": ""nested"",
                ""Precision"": 5,
                ""relativeUrls"": true,
                ""sourceMapRoot"": """",
                ""lineFeed"": """",
                ""sourceMap"": false
            },
        },
        ""minifiers"": {
            ""css"": {
                ""enabled"": true,
                ""termSemicolons"": true,
                ""gzip"": true
            },
            ""javascript"": {
                ""enabled"": true,
                ""termSemicolons"": true,
                ""gzip"": false
            }
        }
    }
".Trim()
                );
            }
        }
        private static int Main(params string[] args)
        {
            return Execute(args);
        }

        public static int Execute(string[] args)
        {
            if (!args.Any() || args.Contains("-h") || args.Contains("--help"))
            {
                ShowHelp(show_file_help: args.Contains("-f") || args.Contains("--files"), show_config_help: args.Contains("-c") || args.Contains("--config"));
                return 0;
            }
            if (args.Contains("-d") || args.Contains("--defaults"))
            {
                CreateDefaultConfig(args.ToList());
                return 0;
            }
            var config = GetConfig(args.ToList());
            if (config == null)
            {
                return 1;
            }
            var compilers = new Compilers(config);
            var file_arguments = GetFileArguments(args.ToList());
            var recurse = args.Contains("-r") || args.Contains("--recursive");

            foreach (var item in file_arguments)
            {
                if ((File.GetAttributes(item) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    if (!recurse)
                    {
                        Console.WriteLine($"{item} is a directory, but option -r is not used. Ignoring {item} and all items in it.");
                    }
                    foreach (var file in Recurse(item))
                    {
                        var result = compilers.TryCompile(file);
                        if (result.Errors != null)
                        {
                            PrintErrors(result.Errors);
                            if (result.Errors.Any(e => !e.IsWarning))
                            {
                                return 1;
                            }
                        }
                    }
                }
                else
                {
                    var result = compilers.TryCompile(item);
                    if (result.Errors != null)
                    {
                        PrintErrors(result.Errors);
                        if (result.Errors.Any(e => !e.IsWarning))
                        {
                            return 1;
                        }
                    }
                }
            }
            return 0;
        }

        private static void PrintErrors(List<CompilerError> errors)
        {
            foreach (var error in errors)
            {
                Console.WriteLine(error.Message);
            }
        }

        private static IEnumerable<string> Recurse(string directory)
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                if (!Path.GetFileName(file).StartsWith('_'))
                {
                    yield return file;
                }
            }
            foreach (var subdir in Directory.GetDirectories(directory))
            {
                foreach (var file in Recurse(subdir))
                {
                    yield return file;
                }
            }
        }

        private static readonly List<string> other_options = new List<string>
        {
            "-d", "--defaults",
            "-h", "--help"
        };
        private static readonly List<string> options_with_arguments = new List<string>
        {
            "-c", "--config",
            "-f", "--files",
            "-m", "--minify",
            "-z", "--zip"
        };
        private static readonly List<string> options_without_arguments = new List<string>
        {
            "-r", "--recursive"
        };
        private static IEnumerable<string> GetFileArguments(List<string> args)
        {
            /*
  -c|--config <conf.json>        Specify a configuration file for compilation.
  -d|--defaults [conf.json]      Write a default configuration file (file name is webcompilerconfiguration.json, if none is specified).
  -f|--files <files.conf>        Specify a list of files that should be compiled.
  -h|--help                      Show command line help.
  -m|--minify [disable/enable]   Enable/disable minification (default: enabled), ignored if configuration file is provided.
  -r|--recursive                 Recursively search folders for compilable files (only if any of the provided arguments is a folder).
  -z|--zip [disable/enable]      Enable/disable gzip (default: enabled), ignored if configuration file is provided.
             */
            if (other_options.Intersect(args).Any())
            {
                throw new ArgumentException("Invalid handling of arguments (program flow went past helper actions). Please report as a bug.");
            }
            for (var i = 0; i < args.Count; ++i)
            {
                if (options_with_arguments.Contains(args[i]))
                {
                    // ignore this item and the next
                    ++i;
                    continue;
                }
                if (options_without_arguments.Contains(args[i]))
                {
                    // ignore this item
                    continue;
                }
                // this seems to be a file or folder.
                yield return args[i];
            }
            if (args.Contains("-f") || args.Contains("--files"))
            {
                var arg_index = args.IndexOfAny("-f", "--files") + 1;
                if (arg_index >= args.Count)
                {
                    Console.WriteLine("Argument missing for option -f. Did you mean to add a list of files?");
                }
                var file = args[arg_index];
                if (!File.Exists(file))
                {
                    Console.WriteLine($"File {file} not found");
                    yield break;
                }
                foreach (var item in File.ReadAllLines(file, Compiler.Encoding))
                {
                    yield return item;
                }
            }
        }

        private static Config? GetConfig(List<string> args)
        {
            if (args.Contains("-c") || args.Contains("--config"))
            {
                return GetConfigFromFile(args);
            }
            var config = new Config();
            if (IsMinificationDisabled(args))
            {
                config.Minifiers.Enabled = false;
            }
            if (IsCompressionDisabled(args))
            {
                config.Minifiers.GZip = false;
            }
            return config;
        }

        private static bool IsCompressionDisabled(List<string> args)
        {
            if (args.Contains("-z") || args.Contains("--zip"))
            {
                var arg_index = args.IndexOfAny("-z", "--zip") + 1;
                if (arg_index >= args.Count)
                {
                    Console.WriteLine("Argument missing for option -z. Did you mean to disable compression?");
                    return false;
                }
                return args[arg_index].StartsWith("d", StringComparison.InvariantCultureIgnoreCase);
            }
            return false;
        }
        private static bool IsMinificationDisabled(List<string> args)
        {
            if (args.Contains("-m") || args.Contains("--minify"))
            {
                var arg_index = args.IndexOfAny("-m", "--minify") + 1;
                if (arg_index >= args.Count)
                {
                    Console.WriteLine("Argument missing for option -m. Did you mean to disable minification?");
                    return false;
                }
                return args[arg_index].StartsWith("d", StringComparison.InvariantCultureIgnoreCase);
            }
            return false;
        }

        private static Config? GetConfigFromFile(List<string> args)
        {
            try
            {
                var default_index = args.IndexOfAny("-c", "--config");
                var file_arg_index = default_index + 1;
                if (file_arg_index < args.Count && !args[file_arg_index].StartsWith("-"))
                {
                    var config_path = args[file_arg_index];
                    return JsonSerializer.Deserialize<Config>(File.ReadAllText(config_path, Compiler.Encoding), json_serializer_options);
                }
                else
                {
                    Console.WriteLine("Error reading configuration from file: no file specified");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reading configuration from file: {e.Message}");
                return null;
            }
        }
        private static void CreateDefaultConfig(List<string> args)
        {
            var default_index = args.IndexOfAny("-d", "--defaults");
            var file_arg_index = default_index + 1;
            var config_path = "webcompilerconfiguration.json";
            if (file_arg_index < args.Count && !args[file_arg_index].StartsWith("-"))
            {
                config_path = args[file_arg_index];
            }
            File.WriteAllText(config_path, JsonSerializer.Serialize(new Config(), json_serializer_options));
        }
    }
}
