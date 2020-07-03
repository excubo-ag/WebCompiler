using System.Collections.Generic;
using System.Linq;

namespace WebCompiler.Compile
{
    public class PassThroughCompiler : Compiler
    {
        public override CompilerResult Compile(List<(string File, bool Created)> file_sequence)
        {
            var (file, created) = file_sequence.Last();
            return new CompilerResult
            {
                OutputFile = file,
                Created = created
            };
        }
    }
}