using System;
using System.IO;
using System.Text.Json;

namespace WebCompiler
{
    internal class SassCompiler : ICompiler
    {
        private string _output = string.Empty;
        private readonly string _error = string.Empty;

        public class RawCompilerError
        {
            public string message { get; set; }
            public int column { get; set; }
            public int line { get; set; }
        }

        public CompilerResult Compile(Config config)
        {
            var baseFolder = Path.GetDirectoryName(config.FileName);
            var inputFile = Path.Combine(baseFolder, config.InputFile);

            var info = new FileInfo(inputFile);
            var content = File.ReadAllText(info.FullName);

            var result = new CompilerResult
            {
                FileName = info.FullName,
                OriginalContent = content,
            };

            try
            {
                RunCompilerProcess(config, info);

                var sourceMapIndex = _output.LastIndexOf("*/");
                if (sourceMapIndex > -1 && _output.Contains("sourceMappingURL=data:"))
                {
                    _output = _output.Substring(0, sourceMapIndex + 2);
                }

                result.CompiledContent = _output;

                if (_error.Length > 0)
                {
                    var json = JsonSerializer.Deserialize<RawCompilerError>(_error);

                    var ce = new CompilerError
                    {
                        FileName = info.FullName,
                        Message = json.message,
                        ColumnNumber = json.column,
                        LineNumber = json.line,
                        IsWarning = !string.IsNullOrEmpty(_output)
                    };

                    result.Errors.Add(ce);
                }
            }
            catch (Exception ex)
            {
                var error = new CompilerError
                {
                    FileName = info.FullName,
                    Message = string.IsNullOrEmpty(_error) ? ex.Message : _error,
                    LineNumber = 0,
                    ColumnNumber = 0,
                };

                result.Errors.Add(error);
            }

            return result;
        }

        private void RunCompilerProcess(Config config, FileInfo info)
        {
            var arguments = ConstructArguments(config);
            // TODO see whether more arguments need to be passed to SassCompiler
            var inline_source_map = config.Compilers.Sass.SourceMap;
            var result = LibSassHost.SassCompiler.CompileFile(info.FullName, config.GetAbsoluteOutputFile().FullName, info.Name, new LibSassHost.CompilationOptions
            {
                SourceMap = true,
                InlineSourceMap = inline_source_map
            });
            _output = result.CompiledContent;
            //ProcessStartInfo start = new ProcessStartInfo
            //{
            //    WorkingDirectory = new FileInfo(config.FileName).DirectoryName, // use config's directory to fix source map relative paths
            //    UseShellExecute = false,
            //    WindowStyle = ProcessWindowStyle.Hidden,
            //    CreateNoWindow = true,
            //    FileName = "cmd.exe",
            //    Arguments = $"/c \"\"{Path.Combine(_path, "node_modules\\.bin\\node-sass.cmd")}\" {arguments} \"{info.FullName}\" \"",
            //    StandardOutputEncoding = Encoding.UTF8,
            //    StandardErrorEncoding = Encoding.UTF8,
            //    RedirectStandardOutput = true,
            //    RedirectStandardError = true,
            //};

            //// Pipe output from node-sass to postcss if autoprefix option is set
            //SassOptions options = SassOptions.FromConfig(config);
            //if (!string.IsNullOrEmpty(options.autoPrefix))
            //{
            //    string postCssArguments = "--use autoprefixer";

            //    if (!options.sourceMap && !config.sourceMap)
            //        postCssArguments += " --no-map";

            //    start.Arguments = start.Arguments.TrimEnd('"') + $" | \"{Path.Combine(_path, "node_modules\\.bin\\postcss.cmd")}\" {postCssArguments}\"";
            //    start.EnvironmentVariables.Add("BROWSERSLIST", options.autoPrefix);
            //}

            //start.EnvironmentVariables["PATH"] = _path + ";" + start.EnvironmentVariables["PATH"];

            //using (Process p = Process.Start(start))
            //{
            //    var stdout = p.StandardOutput.ReadToEndAsync();
            //    var stderr = p.StandardError.ReadToEndAsync();
            //    p.WaitForExit();

            //    _output = stdout.Result;
            //    // postcss outputs "√ Finished stdin (##ms)" to stderr for some reason
            //    if (!stderr.Result.StartsWith("√"))
            //        _error = stderr.Result;
            //}
        }

        private static string ConstructArguments(Config config)
        {
            var arguments = "";

            var options = config.Compilers.Sass;

            if (options.SourceMap || config.SourceMap)
            {
                arguments += " --source-map-embed=true";
            }

            arguments += " --precision=" + options.Precision;

            if (!string.IsNullOrEmpty(options.OutputStyle))
            {
                arguments += " --output-style=" + options.OutputStyle;
            }

            if (options.IndentType != "space")
            {
                arguments += " --indent-type=" + options.IndentType.ToLowerInvariant();
            }

            if (options.IndentWidth > -1)
            {
                arguments += " --indent-width=" + options.IndentWidth;
            }

            if (!string.IsNullOrEmpty(options.IncludePath))
            {
                arguments += " --include-path=" + options.IncludePath;
            }

            if (!string.IsNullOrEmpty(options.SourceMapRoot))
            {
                arguments += " --source-map-root=" + options.SourceMapRoot;
            }

            arguments += " --linefeed=" + options.LineFeed.ToString();

            return arguments;
        }
    }
}
