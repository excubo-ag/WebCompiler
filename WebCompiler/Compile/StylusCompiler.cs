using System;
using System.IO;
using System.Text.RegularExpressions;

namespace WebCompiler
{
    internal class StylusCompiler : ICompiler
    {
        private static readonly Regex _errorRx = new Regex(@"(?<severity>Error|Warning): (?<file>.+):(?<line>[0-9]+):(?<column>[0-9]+).+^(?=[^\s])(?<message>([^\r\n]+))", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);
        private readonly string _path;
        private readonly string _output = string.Empty;
        private readonly string _error = string.Empty;

        public StylusCompiler(string path)
        {
            _path = path;
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

            //ProcessStartInfo start = new ProcessStartInfo
            //{
            //    WorkingDirectory = info.Directory.FullName,
            //    UseShellExecute = false,
            //    WindowStyle = ProcessWindowStyle.Hidden,
            //    CreateNoWindow = true,
            //    FileName = "cmd.exe",
            //    Arguments = $"/c \"\"{Path.Combine(_path, "node_modules\\.bin\\stylus.cmd")}\" {arguments} \"{info.FullName}\"\"",
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
            var arguments = " --print";

            var options = config.Compilers.Stylus;

            if (options.SourceMap || config.SourceMap)
            {
                arguments += " --sourcemap-inline";
            }

            return arguments;
        }
    }
}
