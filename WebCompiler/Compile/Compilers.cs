using System;
using System.IO;
using WebCompiler.Configuration;

namespace WebCompiler.Compile
{
    public class Compilers
    {
        private static readonly Compiler terminating_compiler = new PassThroughCompiler();
        private static readonly Compiler unsupported_compiler = new UnsupportedCompiler();
        private readonly Compiler sass;
        private readonly Compiler css_minifier;
        private readonly Compiler js_minifier;
        private readonly Compiler zipper;
        private readonly Compiler place;
        private readonly Compiler cleanup;

        public Compilers(Config config, string base_path)
        {
            sass = new SassCompiler(config.CompilerSettings.Sass);
            css_minifier = config.Minifiers.Enabled ? new CssMinifier(config.Minifiers.Css) : terminating_compiler;
            js_minifier = config.Minifiers.Enabled ? new JavascriptMinifier(config.Minifiers.Javascript) : terminating_compiler;
            zipper = config.Minifiers.GZip ? new Zipper() : terminating_compiler;
            place = config.Output.Directory != null ? new Place(config.Output.Directory, base_path) : terminating_compiler;
            cleanup = !config.Output.Preserve ? new Cleaner() : terminating_compiler;
        }
        private static CompilationStep Compile(string file) => new CompilationStep(file);
        public CompilationStep TryCompile(string file)
        {
            switch (Path.GetExtension(file).ToUpperInvariant())
            {
                case ".SCSS":
                case ".SASS":
                    return Compile(file).With(sass).Then(css_minifier).Then(zipper).Then(place).Then(cleanup);

                case ".CSS" // we minify (and potentially gzip) .css files, if they are not created by webcompiler
                when css_minifier != terminating_compiler
                && !file.EndsWith(".min.css", StringComparison.InvariantCultureIgnoreCase):
                    return Compile(file).With(css_minifier).Then(zipper).Then(place).Then(cleanup);

                case ".JS" // we minify (and potentially gzip) .js files, if they are not created by webcompiler
                when js_minifier != terminating_compiler
                && !file.EndsWith(".min.js", StringComparison.InvariantCultureIgnoreCase):
                    return Compile(file).With(js_minifier).Then(zipper).Then(place).Then(cleanup);

                case ".CSS" //we zip .css files, if they are .min.css files that are not created by webcompiler
                when zipper != terminating_compiler
                && file.EndsWith(".min.css", StringComparison.InvariantCultureIgnoreCase)
                && !File.Exists(file.Replace(".min.css", ".css", StringComparison.InvariantCultureIgnoreCase)):
                    return Compile(file).With(zipper).Then(place).Then(cleanup);

                case ".JS" //we zip .js files, if they are .min.js files that are not created by webcompiler
                when zipper != terminating_compiler
                && file.EndsWith(".min.js", StringComparison.InvariantCultureIgnoreCase)
                && !File.Exists(file.Replace(".min.js", ".js", StringComparison.InvariantCultureIgnoreCase)):
                    return Compile(file).With(zipper).Then(place).Then(cleanup);

                case ".LESS":
                case ".HANDLEBARS":
                case ".HBS":
                case ".STYL":
                case ".STYLUS":
                case ".COFFEE":
                case ".ICED":
                case ".JSX":
                case ".ES6":
                    return Compile(file).With(unsupported_compiler);
                default:
                    return Compile(file).With(terminating_compiler);
            }
        }
    }
}