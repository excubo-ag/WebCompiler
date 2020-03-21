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
            var ext = Path.GetExtension(inputFile).ToUpperInvariant();

            return AllowedExtensions.Contains(ext);
        }

        internal static ICompiler GetCompiler(Config config)
        {
            var ext = Path.GetExtension(config.InputFile).ToUpperInvariant();
            return ext switch
            {
                ".LESS" => new LessCompiler(_path),
                ".HANDLEBARS" => new HandlebarsCompiler(_path),
                ".HBS" => new HandlebarsCompiler(_path),
                ".SCSS" => new SassCompiler(_path),
                ".SASS" => new SassCompiler(_path),
                ".STYL" => new StylusCompiler(_path),
                ".STYLUS" => new StylusCompiler(_path),
                ".COFFEE" => new IcedCoffeeScriptCompiler(_path),
                ".ICED" => new IcedCoffeeScriptCompiler(_path),
                ".JS" => new BabelCompiler(_path),
                ".JSX" => new BabelCompiler(_path),
                ".ES6" => new BabelCompiler(_path),
                _ => throw new NotSupportedException($"No compiler found for file type {ext}")
            };
        }
    }
}
