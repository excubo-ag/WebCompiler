using System.Collections.Generic;
using System.Linq;

namespace WebCompiler.Compile
{
    public class CompilationStep
    {
        public string InputFile { get; set; }
        public string? OutputFile { get; set; }
        public List<CompilerError>? Errors { get; set; }
        public CompilationStep(string file)
        {
            InputFile = file;
        }
        public CompilationStep With(Compiler compiler)
        {
            var result = compiler.Compile(InputFile);
            if (result.Errors != null)
            {
                Errors = result.Errors;
            }
            OutputFile = result.OutputFile;
            return this;
        }
        public CompilationStep Then(Compiler compiler)
        {
            if (Errors != null && Errors.Any(e => !e.IsWarning))
            {
                return this;
            }
            if (OutputFile == null)
            {
                Errors ??= new List<CompilerError>();
                Errors.Add(new CompilerError
                {
                    FileName = InputFile,
                    Message = "A compilation step did not produce an output file"
                });
                return this;
            }
            InputFile = OutputFile;
            if (string.IsNullOrEmpty(InputFile))
            {
                Errors ??= new List<CompilerError>();
                Errors.Add(new CompilerError
                {
                    Message = "A previous compilation step did not produce an output file"
                });
                return this;
            }
            return With(compiler);
        }
    }
}