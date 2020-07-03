using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebCompiler.Compile
{
    public class Cleaner : Compiler
    {
        public override CompilerResult Compile(List<(string File, bool Created)> file_sequence)
        {
            var output_file = file_sequence.Last().File;
            var generated_files = file_sequence
                .SkipLast(1) // ignore the last file, see next line
                .Append((File: output_file, Created: false)) // pretend that the last file wasn't created by the compiler to prevent deleting it
                .Select(e => (File: Path.GetFullPath(e.File), Created: e.Created)) // make paths uniformily FullPath
                .GroupBy(e => e.File) // see whether any file occurs multiple times
                .Where(g => g.All(e => e.Created)) // only take those where all entries say that the file was created by the compiler
                .Select(g => g.Key) // take the file name
                .ToList();
            foreach (var file in generated_files)
            {
                File.Delete(file);
            }
            return new CompilerResult
            {
                OutputFile = file_sequence.Last().File
            };
        }
    }
}
