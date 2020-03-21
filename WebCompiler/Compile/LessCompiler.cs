using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebCompiler
{
    internal class LessCompiler : ICompiler
    {
        private static readonly Regex _errorRx = new Regex("(?<message>.+) on line (?<line>[0-9]+), column (?<column>[0-9]+)", RegexOptions.Compiled);
        private string _output = string.Empty;
        private string _error = string.Empty;

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

                result.CompiledContent = _output;

                if (_error.Length > 0)
                {
                    var ce = new CompilerError
                    {
                        FileName = info.FullName,
                        Message = _error.Replace(baseFolder, string.Empty),
                        IsWarning = !string.IsNullOrEmpty(_output)
                    };

                    var match = _errorRx.Match(_error);

                    if (match.Success)
                    {
                        ce.Message = match.Groups["message"].Value.Replace(baseFolder, string.Empty);
                        ce.LineNumber = int.Parse(match.Groups["line"].Value);
                        ce.ColumnNumber = int.Parse(match.Groups["column"].Value);
                    }

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
            var result = NUglify.Uglify.Css(File.ReadAllText(info.FullName), new NUglify.Css.CssSettings { }, new NUglify.JavaScript.CodeSettings { });
            if (result.HasErrors)
            {
                _error = string.Join("\n", result.Errors.Select(e => e.Message));
            }
            _output = result.Code;
            //ProcessStartInfo start = new ProcessStartInfo
            //{
            //    WorkingDirectory = info.Directory.FullName,
            //    UseShellExecute = false,
            //    WindowStyle = ProcessWindowStyle.Hidden,
            //    CreateNoWindow = true,
            //    FileName = "cmd.exe",
            //    Arguments = $"/c \"\"{Path.Combine(_path, "node_modules\\.bin\\lessc.cmd")}\" {arguments} \"{info.FullName}\"\"",
            //    StandardOutputEncoding = Encoding.UTF8,
            //    StandardErrorEncoding = Encoding.UTF8,
            //    RedirectStandardOutput = true,
            //    RedirectStandardError = true,
            //};

            //start.EnvironmentVariables["PATH"] = _path + ";" + start.EnvironmentVariables["PATH"];

            //using (Process p = Process.Start(start))
            //{
            //    var stdout = p.StandardOutput.ReadToEndAsync();
            //    var stderr = p.StandardError.ReadToEndAsync();
            //    p.WaitForExit();

            //    _output = stdout.Result.Trim();
            //    _error = stderr.Result.Trim();
            //}
        }

        private static string ConstructArguments(Config config)
        {
            var arguments = " --no-color --js";

            var options = config.Compilers.Less;

            if (options.SourceMap || config.SourceMap)
            {
                arguments += " --source-map-map-inline";
            }

            if (options.Math != null)
            {
                arguments += $" --math={options.Math}";
            }
            else if (options.StrictMath)
            {
                arguments += " --math=strict-legacy";
            }

            if (options.IeCompat)
            {
                arguments += " --ie-compat";
            }

            if (options.StrictUnits)
            {
                arguments += " --strict-units=on";
            }

            if (options.RelativeUrls)
            {
                arguments += " --rewrite-urls=all";
            }

            if (!string.IsNullOrEmpty(options.RootPath))
            {
                arguments += $" --rootpath=\"{options.RootPath}\"";
            }

            if (!string.IsNullOrEmpty(options.CssComb) && !options.CssComb.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                arguments += $" --csscomb=\"{options.CssComb}\"";
            }

            if (!string.IsNullOrEmpty(options.SourceMapRoot))
            {
                arguments += " --source-map-rootpath=" + options.SourceMapRoot;
            }

            if (!string.IsNullOrEmpty(options.SourceMapBasePath))
            {
                arguments += " --source-map-basepath=" + options.SourceMapBasePath;
            }

            return arguments;
        }
    }
}
