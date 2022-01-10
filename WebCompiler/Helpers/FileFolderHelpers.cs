using System;
using System.Collections.Generic;
using System.IO;

namespace WebCompiler.Helpers
{
    public static class FileFolderHelpers
    {
        public static IEnumerable<string> Recurse(string directory)
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                if (!Path.GetFileName(file).StartsWith('_'))
                {
                    yield return file;
                }
            }
            foreach (var subdir in Directory.GetDirectories(directory))
            {
                foreach (var file in Recurse(subdir))
                {
                    yield return file;
                }
            }
        }
    }
}