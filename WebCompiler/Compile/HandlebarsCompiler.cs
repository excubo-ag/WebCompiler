using System;
using System.IO;
using System.Text.RegularExpressions;

namespace WebCompiler
{
    internal class HandlebarsCompiler : ICompiler
    {
        private static readonly Regex _errorRx = new Regex("Error: (?<message>.+) on line (?<line>[0-9]+):", RegexOptions.Compiled);
        private string _mapPath;
        private readonly string _path;
        private string _name = string.Empty;
        private string _extension = string.Empty;
        private readonly string _output = string.Empty;
        private readonly string _error = string.Empty;
        private bool _partial = false;

        public HandlebarsCompiler(string path)
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

            var extension = Path.GetExtension(inputFile);
            if (!string.IsNullOrWhiteSpace(extension))
            {
                _extension = extension.Substring(1);
            }

            var name = Path.GetFileNameWithoutExtension(inputFile);
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("_"))
            {
                _name = name.Substring(1);
                _partial = true;

                // Temporarily Fix
                // TODO: Remove after actual fix
                var tempFilename = Path.Combine(Path.GetDirectoryName(inputFile), _name + ".handlebarstemp");
                info.CopyTo(tempFilename);
                info = new FileInfo(tempFilename);
                _extension = "handlebarstemp";
            }

            _mapPath = Path.ChangeExtension(inputFile, ".js.map.tmp");

            try
            {
                RunCompilerProcess(config, info);

                result.CompiledContent = _output;

                var options = config.Compilers.Handlebars;

                if ((options.SourceMap || config.SourceMap) && File.Exists(_mapPath))
                {
                    result.SourceMap = File.ReadAllText(_mapPath);
                }

                if (_error.Length > 0)
                {
                    var ce = new CompilerError
                    {
                        FileName = inputFile,
                        Message = _error.Replace(baseFolder, string.Empty),
                        IsWarning = !string.IsNullOrEmpty(_output)
                    };

                    var match = _errorRx.Match(_error);

                    if (match.Success)
                    {
                        ce.Message = match.Groups["message"].Value.Replace(baseFolder, string.Empty);
                        ce.LineNumber = int.Parse(match.Groups["line"].Value);
                        ce.ColumnNumber = 0;
                    }

                    result.Errors.Add(ce);
                }
            }
            catch (Exception ex)
            {
                var error = new CompilerError
                {
                    FileName = inputFile,
                    Message = string.IsNullOrEmpty(_error) ? ex.Message : _error,
                    LineNumber = 0,
                    ColumnNumber = 0,
                };

                result.Errors.Add(error);
            }
            finally
            {
                if (File.Exists(_mapPath))
                {
                    File.Delete(_mapPath);
                }
                // Temporarily Fix
                // TODO: Remove after actual fix
                if (info.Extension == ".handlebarstemp")
                {
                    info.Delete();
                }
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
            //    Arguments = $"/c \"\"{Path.Combine(_path, "node_modules\\.bin\\handlebars.cmd")}\" \"{info.FullName}\" {arguments}\"",
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

        private string ConstructArguments(Config config)
        {
            var arguments = "";

            var options = config.Compilers.Handlebars;

            if (options.AMD)
            {
                arguments += " --amd";
            }
            else if (!string.IsNullOrEmpty(options.CommonJs))
            {
                arguments += $" --commonjs \"{options.CommonJs}\"";
            }

            foreach (var knownHelper in options.KnownHelpers)
            {
                arguments += $" --known \"{knownHelper}\"";
            }

            if (options.KnownHelpersOnly)
            {
                arguments += " --knownOnly";
            }

            if (options.ForcePartial || _partial)
            {
                arguments += " --partial";
            }

            if (options.NoBOM)
            {
                arguments += " --bom";
            }

            if ((options.SourceMap || config.SourceMap) && !string.IsNullOrWhiteSpace(_mapPath))
            {
                arguments += $" --map \"{_mapPath}\"";
            }

            if (!string.IsNullOrEmpty(options.Namespace))
            {
                arguments += $" --namespace \"{options.Namespace}\"";
            }

            if (!string.IsNullOrEmpty(options.Root))
            {
                arguments += $" --root \"{options.Root}\"";
            }

            if (!string.IsNullOrEmpty(options.Name))
            {
                arguments += $" --name \"{options.Name}\"";
            }
            else if (!string.IsNullOrEmpty(_name))
            {
                arguments += $" --name \"{_name}\"";
            }

            if (!string.IsNullOrEmpty(_extension))
            {
                arguments += $" --extension \"{_extension}\"";
            }

            return arguments;
        }
    }
}
