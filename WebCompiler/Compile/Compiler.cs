using System.IO;
using System.Text;

namespace WebCompiler.Compile
{
    public abstract class Compiler
    {
        public static readonly Encoding Encoding = new UTF8Encoding(true);
        public abstract CompilerResult Compile(string file);
        public static void ReplaceIfNewer(string output_file, string new_content)
        {
            if (!File.Exists(output_file))
            {
                File.WriteAllText(output_file, new_content, Encoding);
                return;
            }
            var old_content = File.ReadAllText(output_file, Encoding);
            if (old_content != new_content)
            {
                File.WriteAllText(output_file, new_content, Encoding);
            }
        }
    }
}