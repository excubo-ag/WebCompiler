using NUglify;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace WebCompiler
{
    /// <summary>
    /// Used by the compilers to minify the output files.
    /// </summary>
    public class FileMinifier
    {
        internal static MinificationResult? MinifyFile(Config config)
        {
            var file = config.GetAbsoluteOutputFile();
            var extension = file.Extension.ToUpperInvariant();

            return extension switch
            {
                ".JS" => MinifyJavaScript(config, file.FullName),
                ".CSS" => MinifyCss(config, file.FullName),
                _ => null,
            };
        }

        private static MinificationResult? MinifyJavaScript(Config config, string file)
        {
            var content = File.ReadAllText(file);
            var settings = config.Minifiers.Javascript.ToCodeSettings();

            if (!config.Minifiers.Enabled)
            {
                return null;
            }

            var minFile = GetMinFileName(file);

            var minifiedJs = Uglify.Js(content, settings);
            var result = minifiedJs.Code;

            var containsChanges = FileHelpers.HasFileContentChanged(minFile, result);

            if (!string.IsNullOrEmpty(result))
            {
                OnBeforeWritingMinFile(file, minFile, containsChanges);

                if (containsChanges)
                {
                    File.WriteAllText(minFile, result, new UTF8Encoding(true));
                }

                OnAfterWritingMinFile(file, minFile, containsChanges);

                GzipFile(config, minFile, containsChanges);
            }

            return new MinificationResult(result, null);
        }

        private static MinificationResult? MinifyCss(Config config, string file)
        {
            var content = File.ReadAllText(file);

            var settings = config.Minifiers.Css.ToCssSettings();

            if (!config.Minifiers.Enabled)
            {
                return null;
            }


            // Remove control characters which AjaxMin can't handle
            content = Regex.Replace(content, @"[\u0000-\u0009\u000B-\u000C\u000E-\u001F]", string.Empty);
            var minifiedCss = Uglify.Css(content, settings);

            var result = minifiedCss.Code;
            var minFile = GetMinFileName(file);
            var containsChanges = FileHelpers.HasFileContentChanged(minFile, result);

            OnBeforeWritingMinFile(file, minFile, containsChanges);

            if (containsChanges)
            {
                File.WriteAllText(minFile, result, new UTF8Encoding(true));
            }

            OnAfterWritingMinFile(file, minFile, containsChanges);

            GzipFile(config, minFile, containsChanges);

            return new MinificationResult(result, null);
        }

        private static string GetMinFileName(string file)
        {
            var ext = Path.GetExtension(file);

            var fileName = file.Substring(0, file.LastIndexOf(ext));
            if (!fileName.EndsWith(".min"))
            {
                fileName += ".min";
            }

            return fileName + ext;
        }

        private static void GzipFile(Config config, string sourceFile, bool containsChanges)
        {
            if (!config.Minifiers.GZip)
            {
                return;
            }

            var gzipFile = sourceFile + ".gz";

            OnBeforeWritingGzipFile(sourceFile, gzipFile, containsChanges);

            if (containsChanges)
            {
                using var sourceStream = File.OpenRead(sourceFile);
                using var targetStream = File.OpenWrite(gzipFile);
                using var gzipStream = new GZipStream(targetStream, CompressionMode.Compress);
                sourceStream.CopyTo(gzipStream);
            }

            OnAfterWritingGzipFile(sourceFile, gzipFile, containsChanges);
        }

        private static void OnBeforeWritingMinFile(string file, string minFile, bool containsChanges)
        {
            BeforeWritingMinFile?.Invoke(null, new MinifyFileEventArgs(file, minFile, containsChanges));
        }

        private static void OnAfterWritingMinFile(string file, string minFile, bool containsChanges)
        {
            AfterWritingMinFile?.Invoke(null, new MinifyFileEventArgs(file, minFile, containsChanges));
        }


        private static void OnBeforeWritingGzipFile(string minFile, string gzipFile, bool containsChanges)
        {
            BeforeWritingGzipFile?.Invoke(null, new MinifyFileEventArgs(minFile, gzipFile, containsChanges));
        }

        private static void OnAfterWritingGzipFile(string minFile, string gzipFile, bool containsChanges)
        {
            AfterWritingGzipFile?.Invoke(null, new MinifyFileEventArgs(minFile, gzipFile, containsChanges));
        }

        /// <summary>
        /// Fires before the minified file is written to disk.
        /// </summary>
        public static event EventHandler<MinifyFileEventArgs> BeforeWritingMinFile;

        /// <summary>
        /// /// Fires after the minified file is written to disk.
        /// </summary>
        public static event EventHandler<MinifyFileEventArgs> AfterWritingMinFile;

        /// <summary>
        /// Fires before the .gz file is written to disk
        /// </summary>
        public static event EventHandler<MinifyFileEventArgs> BeforeWritingGzipFile;

        /// <summary>
        /// Fires after the .gz file is written to disk
        /// </summary>
        public static event EventHandler<MinifyFileEventArgs> AfterWritingGzipFile;
    }
}
