using System;
using System.IO;
using System.Linq;

namespace WebCompiler
{
    /// <summary>
    /// A service for working with the compilers.
    /// </summary>
    public static class CompilerService
    {
        internal const string Version = "1.4.167";
        private static readonly string _path = Path.Combine(Path.GetTempPath(), "WebCompiler" + Version);

        /// <summary>A list of allowed file extensions.</summary>
        public static readonly string[] AllowedExtensions = new[] { ".LESS", ".SCSS", ".SASS", ".STYL", ".COFFEE", ".ICED", ".JS", ".JSX", ".ES6", ".HBS", ".HANDLEBARS" };

        /// <summary>
        /// Test if a file type is supported by the compilers.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns>True if the file type can be compiled.</returns>
        public static bool IsSupported(string inputFile)
        {
            string ext = Path.GetExtension(inputFile).ToUpperInvariant();

            return AllowedExtensions.Contains(ext);
        }

        internal static ICompiler GetCompiler(Config config)
        {
            string ext = Path.GetExtension(config.inputFile).ToUpperInvariant();
            ICompiler compiler = null;

            switch (ext)
            {
                case ".LESS":
                    compiler = new LessCompiler(_path);
                    break;

                case ".HANDLEBARS":
                case ".HBS":
                    compiler = new HandlebarsCompiler(_path);
                    break;

                case ".SCSS":
                case ".SASS":
                    compiler = new SassCompiler(_path);
                    break;

                case ".STYL":
                case ".STYLUS":
                    compiler = new StylusCompiler(_path);
                    break;

                case ".COFFEE":
                case ".ICED":
                    compiler = new IcedCoffeeScriptCompiler(_path);
                    break;

                case ".JS":
                case ".JSX":
                case ".ES6":
                    compiler = new BabelCompiler(_path);
                    break;
            }

            return compiler;
        }
    }
}
