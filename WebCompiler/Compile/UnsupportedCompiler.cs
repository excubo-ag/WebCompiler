using System.Collections.Generic;

namespace WebCompiler.Compile
{
    public class UnsupportedCompiler : Compiler
    {
        public override CompilerResult Compile(string file)
        {
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