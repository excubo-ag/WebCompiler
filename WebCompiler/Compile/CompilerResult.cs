using System.Collections.Generic;

namespace WebCompiler.Compile
{
    /// <summary>
    /// Contains the result of a compilation.
    /// </summary>
    public class CompilerResult
    {
        public string? OutputFile { get; set; }
        public bool Created { get; set; }

        /// <summary>
        /// A collection of any errors reported by the compiler.
        /// </summary>
        public List<CompilerError> Errors { get; set; } = new List<CompilerError>();
    }
}
