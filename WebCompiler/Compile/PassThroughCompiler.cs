namespace WebCompiler.Compile
{
    public class PassThroughCompiler : Compiler
    {
        public override CompilerResult Compile(string file)
        {
            return new CompilerResult
            {
                OutputFile = file
            };
        }
    }
}