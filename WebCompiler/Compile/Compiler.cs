using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WebCompiler.Compile
{
    public abstract class Compiler
    {
        public static readonly Encoding Encoding = new UTF8Encoding(true);
        public abstract CompilerResult Compile(List<(string File, bool Created)> file_sequence);
        public static bool ReplaceIfNewer(string output_file, string new_content)
        {
            if (!File.Exists(output_file))
            {
                File.WriteAllText(output_file, new_content, Encoding);
                return true;
            }
            var old_content = File.ReadAllText(output_file, Encoding);
            if (old_content != new_content)
            {
                File.WriteAllText(output_file, new_content, Encoding);
            }
            return false;
        }
        public static bool ReplaceIfNewer(string output_file, byte[] new_content)
        {
            if (!File.Exists(output_file))
            {
                File.WriteAllBytes(output_file, new_content);
                return true;
            }
            var old_content = File.ReadAllBytes(output_file);
            if (!old_content.SequenceEqual(new_content))
            {
                File.WriteAllBytes(output_file, new_content);
            }
            return false;
        }
    }
}