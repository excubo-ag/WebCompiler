using WebCompiler.Configuration.Settings;
using System.Collections.Generic;

namespace WebCompiler.Configuration
{
    public class CompilerSettingsCollection
    {
        public SassSettings Sass { get; set; } = new SassSettings();

        /// <summary>
        /// Ignore Files or Folders matching certain patterns
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/core/extensions/file-globbing" />
        /// 
        /// If no pattern(s) are specified, by default **/_*.* will be applied to ignore any files
        /// prefixed with _
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <returns></returns>
        public List<string> Ignore { get; set; } = new List<string>(){IgnoreUnderscorePrefixedFilesGlobPattern};

        private const string IgnoreUnderscorePrefixedFilesGlobPattern = "**/_*.*";
    }
}