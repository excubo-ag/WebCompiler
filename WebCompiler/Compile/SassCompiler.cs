using AutoprefixerHost;
using DartSassHost;
using JavaScriptEngineSwitcher.ChakraCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WebCompiler.Configuration;
using WebCompiler.Configuration.Settings;

namespace WebCompiler.Compile
{
    public class SassCompiler : Compiler
    {
        private readonly SassSettings settings;
        private readonly CssAutoprefixSettings autoprefix_settings;

        public SassCompiler(SassSettings settings)
        {
            this.settings = settings;
            autoprefix_settings = new CssAutoprefixSettings
            {
                Enabled = false
            };
        }

        public SassCompiler(SassSettings settings, CssAutoprefixSettings autoprefix_settings)
        {
            this.settings = settings;
            this.autoprefix_settings = autoprefix_settings;
        }

        public override CompilerResult Compile(List<(string File, bool Created)> file_sequence)
        {
            var file = file_sequence.Last().File;
            var tmp_output_file = Path.Combine(Path.GetDirectoryName(file)!, Path.GetFileNameWithoutExtension(file) + ".css.tmp");
            var output_file = Path.Combine(Path.GetDirectoryName(file)!, Path.GetFileNameWithoutExtension(file) + ".css");

            if (File.Exists(output_file))
            {
                var last_write_output = new FileInfo(output_file).LastWriteTimeUtc;
                // determine if this file with all its dependencies is newer than the output file, which necessitates compilation.
                var dependencies = GetDependencies(file);
                var needs_compilation = dependencies.Select(d => new FileInfo(d).LastWriteTimeUtc).Any(last_write_input => last_write_input > last_write_output);

                if (!needs_compilation)
                {
                    return new CompilerResult
                    {
                        OutputFile = output_file
                    };
                }
            }
            try
            {
                var options = new CompilationOptions
                {
                    IndentType = settings.IndentType,
                    IndentWidth = settings.IndentWidth,
                    LineFeedType = settings.LineFeed,
                    OutputStyle = settings.OutputStyle,
                    SourceMap = settings.SourceMap,
                    InlineSourceMap = settings.SourceMap,
                };
                if (settings.IncludePath != null)
                {
                    options.IncludePaths.Add(settings.IncludePath);
                }
                var compiler = new DartSassHost.SassCompiler(new ChakraCoreJsEngineFactory());
                var compile_result = compiler.CompileFile(file, tmp_output_file, file, options);
                var replaced = ReplaceIfNewer(output_file, compile_result.CompiledContent);
                if (!autoprefix_settings.Enabled)
                {
                    return new CompilerResult
                    {
                        OutputFile = output_file,
                        Created = replaced
                    };
                }

                var map_file = Path.Combine(Path.GetDirectoryName(file)!, Path.GetFileNameWithoutExtension(file) + ".css.map");
                using var autoprefixer = new Autoprefixer(new ChakraCoreJsEngineFactory(), autoprefix_settings.ProcessingOptions);
                var result = autoprefixer.Process(compile_result.CompiledContent, output_file, tmp_output_file, map_file, compile_result.SourceMap);
                var autoPrefixerReplaced = ReplaceIfNewer(output_file, result.ProcessedContent);
                return new CompilerResult
                {
                    OutputFile = output_file,
                    Created = replaced || autoPrefixerReplaced
                };

            }
            catch (SassCompilationException ex)
            {
                return new CompilerResult
                {
                    Errors = new List<CompilerError>
                    {
                        new CompilerError
                        {
                            FileName = ex.File,
                            Message = ex.Message,
                            LineNumber = ex.LineNumber,
                            ColumnNumber = ex.ColumnNumber
                        }
                    }
                };
            }
        }

        private List<string> GetDependencies(string file)
        {
            var dependencies = new HashSet<string> { file };
            var candidates = new HashSet<string> { file };
            while (candidates.Any())
            {
                var candidate = candidates.First();
                _ = candidates.Remove(candidate);

                var content = File.ReadAllText(candidate, Encoding);
                //match both <@import "myFile.scss";> and <@import url("myFile.scss");> syntax
                var matches = Regex.Matches(content, @"(?<=@import(?:[\s]+))(?:(?:\(\w+\)))?\s*(?:url)?(?<url>[^;]+)", RegexOptions.Multiline);
                if (matches.Any())
                {
                    var info = new FileInfo(candidate);
                    foreach (var match in matches.Where(m => m != null))
                    {
                        var importedfiles = GetFileInfos(info, match);
                        foreach (var importedfile in importedfiles)
                        {
                            if (dependencies.Add(importedfile)) // only consider the dependency a candidate, if we haven't encountered it yet.
                            {
                                _ = candidates.Add(importedfile);
                            }
                        }
                    }
                }
            }
            return dependencies.ToList();
        }

        private static IEnumerable<string> GetFileInfos(FileInfo info, Match match)
        {
            var url = match.Groups["url"].Value.Replace("'", "\"").Replace("(", "").Replace(")", "").Replace(";", "").Trim();

            foreach (var name in url.Split(new[] { "\"," }, StringSplitOptions.RemoveEmptyEntries))
            {
                var value = name.Replace("\"", "").Replace('/', Path.DirectorySeparatorChar).Trim();

                string file_name = Path.GetFileName(value);
                string file_path = Path.GetDirectoryName(value) ?? string.Empty;

                if (value.EndsWith(".scss", StringComparison.InvariantCultureIgnoreCase) ||
                    value.EndsWith(".sass", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (File.Exists(Path.Combine(info.DirectoryName, value)))
                    {
                        yield return Path.Combine(info.DirectoryName, value);
                    }
                    else if (File.Exists(Path.Combine(info.DirectoryName, file_path, "_" + file_name)))
                    {
                        yield return Path.Combine(info.DirectoryName, file_path, "_" + file_name);
                    }
                }
                else
                {
                    if (File.Exists(Path.Combine(info.DirectoryName, value + ".scss")))
                    {
                        yield return Path.Combine(info.DirectoryName, value + ".scss");
                    }
                    else if (File.Exists(Path.Combine(info.DirectoryName, file_path, "_" + file_name + ".scss")))
                    {
                        yield return Path.Combine(info.DirectoryName, file_path, "_" + file_name + ".scss");
                    }
                }
            }
        }
    }
}