using System.IO;
using System.IO.Compression;
using System.Linq;

namespace WebCompiler.Compile
{
    public class Zipper : Compiler
    {
        public override CompilerResult Compile(string file)
        {
            var output_file = file + ".gz";
            if (File.Exists(output_file) && new FileInfo(file).LastWriteTimeUtc < new FileInfo(output_file).LastWriteTimeUtc)
            {
                return new CompilerResult
                {
                    OutputFile = output_file
                };
            }
            var tmp_file = output_file + ".tmp.gz";
            Zip(file, tmp_file);
            if (!File.Exists(output_file))
            {
                File.Move(tmp_file, output_file);
            }
            else if (!File.ReadAllBytes(output_file).SequenceEqual(File.ReadAllBytes(tmp_file)))
            {
                File.Move(tmp_file, output_file, true);
            }
            else
            {
                File.Delete(tmp_file);
            }
            return new CompilerResult
            {
                OutputFile = output_file
            };
        }
        private static void Zip(string input_file, string output_file)
        {
            using var sourceStream = File.OpenRead(input_file);
            using var targetStream = File.OpenWrite(output_file);
            using var gzipStream = new GZipStream(targetStream, CompressionMode.Compress);
            sourceStream.CopyTo(gzipStream);
            gzipStream.Close();
            
        }
    }
}
