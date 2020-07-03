using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebCompiler.Compile
{
    internal static class StringExtension
    {
        public static string RelativeTo(this string path, string @base)
        {
            return Path.GetRelativePath(@base, Path.GetFullPath(path));
        }
    }
    public class Place : Compiler
    {
        private readonly string output_directory;
        private readonly string base_path;
        public Place(string output_directory, string base_path)
        {
            this.output_directory = output_directory;
            this.base_path = base_path;
        }

        public override CompilerResult Compile(List<(string File, bool Created)> file_sequence)
        {
            var file = file_sequence.Last().File;
            var relative_path = file.RelativeTo(base_path);
            var output_file = Path.GetFullPath(Path.Combine(output_directory, relative_path));
            if (output_file == Path.GetFullPath(file))
            {
                // nothing to move. file is at the correct position already.
                // make sure this file is not marked as intermediate anymore.
                return new CompilerResult
                {
                    OutputFile = output_file
                };
            }
            else
            {
                if (File.Exists(output_file) && new FileInfo(file).LastWriteTimeUtc < new FileInfo(output_file).LastWriteTimeUtc)
                {
                    // the file already exists and is newer than the input file.
                    return new CompilerResult
                    {
                        OutputFile = output_file
                    };
                }
                _ = Directory.CreateDirectory(Path.GetDirectoryName(output_file));
                var created = ReplaceIfNewer(output_file, File.ReadAllBytes(file));
                return new CompilerResult
                {
                    OutputFile = output_file,
                    Created = created
                };
            }
        }
    }
}
