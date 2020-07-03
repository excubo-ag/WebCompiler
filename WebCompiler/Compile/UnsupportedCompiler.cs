using System.Collections.Generic;
using System.Linq;

namespace WebCompiler.Compile
{
    public class UnsupportedCompiler : Compiler
    {
        public override CompilerResult Compile(List<(string File, bool Created)> file_sequence)
        {
            var file = file_sequence.Last().File;
            return new CompilerResult
            {
                Errors = new List<CompilerError>
                {
                    new CompilerError
                    {
                        IsWarning = true,
                        Message = $"Compilation of file {file} is not yet supported"
                    }
                }
            };
        }
    }
}