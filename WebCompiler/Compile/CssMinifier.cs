using NUglify;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WebCompiler.Configuration.Settings;

namespace WebCompiler.Compile
{
    /// <summary>
    /// Used by the compilers to minify the output files.
    /// </summary>
    public class CssMinifier : Minifier
    {
        private readonly CssMinifySettings settings;

        public CssMinifier(CssMinifySettings settings)
        {
            this.settings = settings;
        }
        public override CompilerResult Compile(string file)
        {
            var output_file = GetMinFileName(file);
            if (File.Exists(output_file) && new FileInfo(file).LastWriteTimeUtc < new FileInfo(output_file).LastWriteTimeUtc)
            {
                return new CompilerResult
                {
                    OutputFile = output_file
                };
            }
            var content = File.ReadAllText(file, Encoding);



            // Remove control characters which AjaxMin can't handle
            content = Regex.Replace(content, @"[\u0000-\u0009\u000B-\u000C\u000E-\u001F]", string.Empty);
            var minifiedCss = Uglify.Css(content, settings);

            if (minifiedCss.Errors != null && minifiedCss.Errors.Any())
            {
                return new CompilerResult
                {
                    Errors = minifiedCss.Errors.Select(e => new CompilerError
                    {
                        ColumnNumber = e.StartColumn,
                        FileName = file,
                        IsWarning = !e.IsError,
                        LineNumber = e.StartLine,
                        Message = e.Message
                    }).ToList()
                };
            }

            ReplaceIfNewer(output_file, minifiedCss.Code);

            return new CompilerResult
            {
                OutputFile = output_file
            };
        }
    }
}
