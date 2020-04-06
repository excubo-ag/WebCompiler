using NUglify;
using NUglify.JavaScript;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebCompiler.Configuration.Settings;

namespace WebCompiler.Compile
{
    public class JavascriptMinifier : Minifier
    {
        private readonly JavaScriptMinifySettings settings;

        public JavascriptMinifier(JavaScriptMinifySettings settings)
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

            UglifyResult minifiedJs;
            try
            {
                minifiedJs = Uglify.Js(content, settings);
            }
            catch (Exception e)
            {
                return new CompilerResult
                {
                    Errors = new List<CompilerError>
                    {
                        new CompilerError
                        {
                            FileName = file,
                            Message = e.Message
                        }
                    }
                };
            }

            if (minifiedJs.Errors != null && minifiedJs.Errors.Any())
            {
                return new CompilerResult
                {
                    Errors = minifiedJs.Errors.Select(e => new CompilerError
                    {
                        ColumnNumber = e.StartColumn,
                        FileName = file,
                        IsWarning = !e.IsError,
                        LineNumber = e.StartLine,
                        Message = e.Message
                    }).ToList()
                };
            }

            ReplaceIfNewer(output_file, minifiedJs.Code);

            return new CompilerResult
            {
                OutputFile = output_file
            };
        }
    }
}
