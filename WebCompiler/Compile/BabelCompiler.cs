using System;
using System.IO;
using System.Text.RegularExpressions;

namespace WebCompiler
{
    internal class BabelCompiler : ICompiler
    {
        private static readonly Regex error_rx = new Regex(@".+\.jsx:\s(?<message>.+)\((?<line>[0-9]+):(?<column>[0-9]+)\)", RegexOptions.Compiled);
        private readonly string _path;
        private string _output = string.Empty;
        private readonly string _error = string.Empty;

        public BabelCompiler(string path)
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

                    Match match = error_rx.Match(_error);

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
            _output = File.ReadAllText(info.FullName);

            ////ProcessStartInfo start = new ProcessStartInfo
            ////{
            ////    WorkingDirectory = info.Directory.FullName,
            ////    UseShellExecute = false,
            ////    WindowStyle = ProcessWindowStyle.Hidden,
            ////    CreateNoWindow = true,
            ////    FileName = "cmd.exe",
            ////    Arguments = $"/c \"\"{Path.Combine(_path, "node_modules\\.bin\\babel.cmd")}\" {arguments} \"{info.FullName}\"\"",
            ////    StandardOutputEncoding = Encoding.UTF8,
            ////    StandardErrorEncoding = Encoding.UTF8,
            ////    RedirectStandardOutput = true,
            ////    RedirectStandardError = true,
            ////};

            ////start.EnvironmentVariables["PATH"] = _path + ";" + start.EnvironmentVariables["PATH"];

            ////using (Process p = Process.Start(start))
            ////{
            ////    var stdout = p.StandardOutput.ReadToEndAsync();
            ////    var stderr = p.StandardError.ReadToEndAsync();
            ////    p.WaitForExit();

            ////    _output = stdout.Result;
            ////    _error = stderr.Result;
            ////}
        }

        private static string ConstructArguments(Config config)
        {
            string arguments = $"--presets react --out-file \"\"";

            BabelOptions options = BabelOptions.FromConfig(config);

            if (options.sourceMap || config.sourceMap)
            {
                arguments += " --source-maps inline";
            }

            return arguments;
        }
    }
}
