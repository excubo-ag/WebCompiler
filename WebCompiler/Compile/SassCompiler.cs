using LibSassHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AutoprefixerHost;
using JavaScriptEngineSwitcher.ChakraCore;
using Newtonsoft.Json;
using WebCompiler.Configuration;
using WebCompiler.Configuration.Settings;

namespace WebCompiler.Compile
{
    public class SassCompiler : Compiler
    {
        private readonly SassSettings          settings;
        private readonly CssAutoprefixSettings autoprefixSettings;

        public SassCompiler(SassSettings settings)
        {
            this.settings = settings;
            autoprefixSettings = new CssAutoprefixSettings
            {
                Enabled = false
            };
        }
        
        public SassCompiler(SassSettings settings, CssAutoprefixSettings autoprefixSettings)
        {
            this.settings           = settings;
            this.autoprefixSettings = autoprefixSettings;
        }

        public override CompilerResult Compile(List<(string File, bool Created)> file_sequence)
        {
            var file = file_sequence.Last().File;
            var tmp_output_file = Path.Combine(Path.GetDirectoryName(file)!, Path.GetFileNameWithoutExtension(file) + ".css.tmp");
            var output_file = Path.Combine(Path.GetDirectoryName(file)!, Path.GetFileNameWithoutExtension(file) + ".css");

            if (File.Exists(output_file))
            {
                // determine if this file with all its dependencies is newer than the output file, which necessitates compilation.
                var dependencies = GetDependencies(file).Append(file).ToList();
                var last_write_output = new FileInfo(output_file).LastWriteTimeUtc;
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
                var compile_result = LibSassHost.SassCompiler.CompileFile(file, tmp_output_file, file, new CompilationOptions
                {
                    IndentType = settings.IndentType,
                    IndentWidth = settings.IndentWidth,
                    LineFeedType = settings.LineFeed,
                    OutputStyle = settings.OutputStyle,
                    Precision = settings.Precision,
                    SourceMap = true,
                    InlineSourceMap = settings.SourceMap
                });
                var scssCreated = ReplaceIfNewer(output_file, compile_result.CompiledContent);
                if (!autoprefixSettings.Enabled)
                {
                    return new CompilerResult
                    {
                            OutputFile = output_file,
                            Created    = scssCreated
                    };
                }
                
                var map_file = Path.Combine(Path.GetDirectoryName(file)!, Path.GetFileNameWithoutExtension(file) + ".css.map");
                using var autoprefixer = new Autoprefixer(new ChakraCoreJsEngineFactory(), autoprefixSettings.ProcessingOptions);
                var       result       = autoprefixer.Process(compile_result.CompiledContent, output_file, tmp_output_file, map_file, compile_result.SourceMap);

                return new CompilerResult
                {
                        OutputFile = output_file,
                        Created    = ReplaceIfNewer(output_file, result.ProcessedContent)
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

        private IEnumerable<string> GetDependencies(string file)
        {
            var content = File.ReadAllText(file, Encoding);
            var info = new FileInfo(file);
            //match both <@import "myFile.scss";> and <@import url("myFile.scss");> syntax
            var matches = Regex.Matches(content, @"(?<=@import(?:[\s]+))(?:(?:\(\w+\)))?\s*(?:url)?(?<url>[^;]+)", RegexOptions.Multiline);
            foreach (var match in matches.Where(m => m != null))
            {
                var importedfiles = GetFileInfos(info, match);
                foreach (var importedfile in importedfiles)
                {
                    yield return importedfile;
                    foreach (var dependency in GetDependencies(importedfile))
                    {
                        yield return dependency;
                    }
                }
            }
        }

        private static IEnumerable<string> GetFileInfos(FileInfo info, Match match)
        {
            var url = match.Groups["url"].Value.Replace("'", "\"").Replace("(", "").Replace(")", "").Replace(";", "").Trim();

            foreach (var name in url.Split(new[] { "\"," }, StringSplitOptions.RemoveEmptyEntries))
            {
                var value = name.Replace("\"", "").Replace('/', Path.DirectorySeparatorChar).Trim();
                if (value.EndsWith(".scss", StringComparison.InvariantCultureIgnoreCase) ||
                    value.EndsWith(".sass", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (File.Exists(Path.Combine(info.DirectoryName, value)))
                    {
                        yield return Path.Combine(info.DirectoryName, value);
                    }
                    else if (File.Exists(Path.Combine(info.DirectoryName, "_" + value)))
                    {
                        yield return Path.Combine(info.DirectoryName, "_" + value);
                    }
                }
                else
                {
                    if (File.Exists(Path.Combine(info.DirectoryName, value + ".scss")))
                    {
                        yield return Path.Combine(info.DirectoryName, value + ".scss");
                    }
                    else if (File.Exists(Path.Combine(info.DirectoryName, "_" + value + ".scss")))
                    {
                        yield return Path.Combine(info.DirectoryName, "_" + value + ".scss");
                    }
                }
            }
        }
    }
}
