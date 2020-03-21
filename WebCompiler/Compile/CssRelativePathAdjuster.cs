using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebCompiler
{
    internal static class CssRelativePath
    {
        private static readonly Regex _rxUrl = new Regex(@"url\s*\(\s*([""']?)([^:)]+)\1\s*\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string Adjust(string content, Config config)
        {
            var cssFileContents = content;
            var absoluteOutputPath = config.GetAbsoluteOutputFile().FullName;

            // apply the RegEx to the file (to change relative paths)
            var matches = _rxUrl.Matches(cssFileContents);

            // Ignore the file if no match
            if (matches.Count > 0)
            {
                var cssDirectoryPath = config.GetAbsoluteInputFile().DirectoryName;

                if (!Directory.Exists(cssDirectoryPath))
                {
                    return cssFileContents;
                }

                foreach (var match in matches.Where(m => m != null))
                {
                    var quoteDelimiter = match.Groups[1].Value; //url('') vs url("")
                    var relativePathToCss = match.Groups[2].Value;

                    // Ignore root relative references
                    if (relativePathToCss.StartsWith("/", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    //prevent query string from causing error
                    var pathAndQuery = relativePathToCss.Split(new[] { '?' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    var pathOnly = pathAndQuery[0];
                    var queryOnly = pathAndQuery.Length == 2 ? pathAndQuery[1] : string.Empty;

                    var absolutePath = GetAbsolutePath(cssDirectoryPath, pathOnly);

                    if (string.IsNullOrEmpty(absoluteOutputPath) || string.IsNullOrEmpty(absolutePath))
                    {
                        continue;
                    }

                    var serverRelativeUrl = FileHelpers.MakeRelative(absoluteOutputPath, absolutePath);

                    if (!string.IsNullOrEmpty(queryOnly))
                    {
                        serverRelativeUrl += "?" + queryOnly;
                    }

                    var replace = string.Format("url({0}{1}{0})", quoteDelimiter, serverRelativeUrl);

                    cssFileContents = cssFileContents.Replace(match.Groups[0].Value, replace);
                }
            }

            return cssFileContents;
        }

        private static string? GetAbsolutePath(string cssFilePath, string pathOnly)
        {
            var invalids = Path.GetInvalidPathChars();

            foreach (var invalid in invalids)
            {
                if (pathOnly.IndexOf(invalid) > -1)
                {
                    return null;
                }
            }

            return Path.GetFullPath(Path.Combine(cssFilePath, pathOnly));
        }
    }
}