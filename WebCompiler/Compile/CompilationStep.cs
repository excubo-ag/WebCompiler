using System.Collections.Generic;
using System.Linq;

namespace WebCompiler.Compile
{
    public class CompilationStep
    {
        public string InputFile { get; set; }
        public string? OutputFile { get; set; }
        public List<CompilerError>? Errors { get; set; }
        public List<(string File, bool Created)> AllFiles { get; set; } = new List<(string File, bool Created)>();
        public CompilationStep(string file)
        {
            InputFile = file;
            AllFiles.Add((File: InputFile, Created: false));
        }
        public CompilationStep With(Compiler compiler)
        {
            var result = compiler.Compile(AllFiles);
            if (result.Errors != null)
            {
                if (Errors == null)
                {
                    Errors = new List<CompilerError>();
                }
                Errors.AddRange(result.Errors);
            }
            OutputFile = result.OutputFile;
            AllFiles.Add((File: OutputFile!, Created: result.Created));
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