using AutoprefixerHost;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WebCompiler.Configuration;

namespace WebCompiler.Compile
{
    public class CssAutoprefixer : Compiler
    {
        private readonly CssAutoprefixSettings settings;

        public CssAutoprefixer(CssAutoprefixSettings settings)
        {
            this.settings = settings;
        }

        public override CompilerResult Compile(List<(string File, bool Created)> file_sequence)
        {
            var file = file_sequence.Last().File;
            var tmp_output_file = Path.Combine(Path.GetDirectoryName(file)!, Path.GetFileNameWithoutExtension(file) + ".css.tmp");
            var map_file = Path.Combine(Path.GetDirectoryName(file)!, Path.GetFileNameWithoutExtension(file) + ".css.map");
            var output_file = Path.Combine(Path.GetDirectoryName(file)!, Path.GetFileNameWithoutExtension(file) + ".css");

            if (File.Exists(output_file) && HasBeenAutoprefixed(output_file))
            {
                return new CompilerResult
                {
                    OutputFile = output_file
                };
            }

            try
            {
                using var autoprefixer = new Autoprefixer(settings.ProcessingOptions);
                var result = autoprefixer.Process(File.ReadAllText(file), file, tmp_output_file, map_file, string.Empty);

                var created = ReplaceIfNewer(output_file, result.ProcessedContent);
                return new CompilerResult
                {
                    OutputFile = output_file,
                    Created = created
                };
            }
            catch (AutoprefixerProcessingException ex)
            {
                return new CompilerResult
                {
                    Errors = new List<CompilerError>
                        {
                                new CompilerError
                                {
                                        FileName     = ex.File,
                                        Message      = ex.Message,
                                        LineNumber   = ex.LineNumber,
                                        ColumnNumber = ex.ColumnNumber
                                }
                        }
                };
            }
        }

        private bool HasBeenAutoprefixed(string file) =>
                Regex.IsMatch(File.ReadAllText(file, Encoding), @"\/\*\# sourceMappingURL(.*).css.map");
    }
}