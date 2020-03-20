using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebCompiler
{
    internal class LessCompiler : ICompiler
    {
        private static readonly Regex _errorRx = new Regex("(?<message>.+) on line (?<line>[0-9]+), column (?<column>[0-9]+)", RegexOptions.Compiled);
        private readonly string _path;
        private string _output = string.Empty;
        private string _error = string.Empty;

        public LessCompiler(string path)
        {
            _path = path;
        }

        public CompilerResult Compile(Config config)
        {
            string baseFolder = Path.GetDirectoryName(config.FileName);
            string inputFile = Path.Combine(baseFolder, config.inputFile);

            FileInfo info = new FileInfo(inputFile);
            string content = File.ReadAllText(info.FullName);

            CompilerResult result = new CompilerResult
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
                    CompilerError ce = new CompilerError
                    {
                        FileName = info.FullName,
                        Message = _error.Replace(baseFolder, string.Empty),
                        IsWarning = !string.IsNullOrEmpty(_output)
                    };

                    Match match = _errorRx.Match(_error);

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
                CompilerError error = new CompilerError
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
            string arguments = ConstructArguments(config);
            NUglify.UglifyResult result = NUglify.Uglify.Css(File.ReadAllText(info.FullName), new NUglify.Css.CssSettings { }, new NUglify.JavaScript.CodeSettings { });
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
            string arguments = " --no-color --js";

            LessOptions options = LessOptions.FromConfig(config);

            if (options.sourceMap || config.sourceMap)
            {
                arguments += " --source-map-map-inline";
            }

            if (options.math != null)
            {
                arguments += $" --math={options.math}";
            }
            else if (options.strictMath)
            {
                arguments += " --math=strict-legacy";
            }

            if (options.ieCompat)
            {
                arguments += " --ie-compat";
            }

            if (options.strictUnits)
            {
                arguments += " --strict-units=on";
            }

            if (options.relativeUrls)
            {
                arguments += " --rewrite-urls=all";
            }

            if (!string.IsNullOrEmpty(options.rootPath))
            {
                arguments += $" --rootpath=\"{options.rootPath}\"";
            }

            if (!string.IsNullOrEmpty(options.autoPrefix))
            {
                arguments += $" --autoprefix=\"{options.autoPrefix}\"";
            }

            if (!string.IsNullOrEmpty(options.cssComb) && !options.cssComb.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                arguments += $" --csscomb=\"{options.cssComb}\"";
            }

            if (!string.IsNullOrEmpty(options.sourceMapRoot))
            {
                arguments += " --source-map-rootpath=" + options.sourceMapRoot;
            }

            if (!string.IsNullOrEmpty(options.sourceMapBasePath))
            {
                arguments += " --source-map-basepath=" + options.sourceMapBasePath;
            }

            return arguments;
        }
    }
}
