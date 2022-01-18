using Microsoft.Extensions.FileSystemGlobbing;
using System;
using System.Collections.Generic;
using System.IO;

namespace WebCompiler.Helpers
{
    public static class FileFolderHelpers
    {
        public static IEnumerable<string> Recurse(string directory, List<string> ignoreGlobs)
        {
            var matcher = new Matcher();
            matcher.AddInclude("**/*");
            matcher.AddExcludePatterns(ignoreGlobs);
            foreach (string file in matcher.GetResultsInFullPath(directory))
            {
                yield return file;
            }
        }
    }
}