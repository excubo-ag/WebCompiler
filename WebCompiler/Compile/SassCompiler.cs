using LibSassHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WebCompiler.Configuration.Settings;

namespace WebCompiler.Compile
{
    public class SassCompiler : Compiler
    {
        private readonly SassSettings settings;

        public SassCompiler(SassSettings settings)
        {
            this.settings = settings;
        }
        public override CompilerResult Compile(string file)
        {
            var tmp_output_file = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".css.tmp");
            var output_file = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".css");

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
                ReplaceIfNewer(output_file, compile_result.CompiledContent);
                return new CompilerResult
                {
                    OutputFile = output_file
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
