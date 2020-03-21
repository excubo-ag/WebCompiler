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
                ".LESS" => new LessCompiler(),
                ".HANDLEBARS" => new HandlebarsCompiler(),
                ".HBS" => new HandlebarsCompiler(),
                ".SCSS" => new SassCompiler(),
                ".SASS" => new SassCompiler(),
                ".STYL" => new StylusCompiler(),
                ".STYLUS" => new StylusCompiler(),
                ".COFFEE" => new IcedCoffeeScriptCompiler(),
                ".ICED" => new IcedCoffeeScriptCompiler(),
                ".JS" => new BabelCompiler(),
                ".JSX" => new BabelCompiler(),
                ".ES6" => new BabelCompiler(),
                _ => throw new NotSupportedException($"No compiler found for file type {ext}")
            };
        }
    }
}
