using System;
using System.IO;
using System.Linq;

namespace WebCompiler
{
    /// <summary>
    /// Helper class for file interactions
    /// </summary>
    public static class FileHelpers
    {
        /// <summary>
        /// Finds the relative path between two files.
        /// </summary>
        public static string MakeRelative(string baseFile, string file)
        {
            var base_segments = baseFile.Split(Path.DirectorySeparatorChar);
            var file_segments = file.Split(Path.DirectorySeparatorChar);
            var i = 0;
            for (; i < base_segments.Length && i < file_segments.Length; ++i)
            {
                if (base_segments[i] != file_segments[i])
                {
                    break;
                }
            }
            var relative_portion_base = string.Join(string.Empty, Enumerable.Repeat(".." + Path.DirectorySeparatorChar, base_segments.Length - i - 1));
            var relative_portion_file = string.Join(string.Empty, file_segments.Skip(i));
            return Uri.UnescapeDataString(relative_portion_base + relative_portion_file);
        }

        /// <summary>
        /// If a file has the read-only attribute, this method will remove it.
        /// </summary>
        public static void RemoveReadonlyFlagFromFile(string fileName)
        {
            var file = new FileInfo(fileName);

            if (file.Exists && file.IsReadOnly)
            {
                file.IsReadOnly = false;
            }
        }

        /// <summary>
        /// If a file has the read-only attribute, this method will remove it.
        /// </summary>
        public static void RemoveReadonlyFlagFromFile(FileInfo file)
        {
            RemoveReadonlyFlagFromFile(file.FullName);
        }

        /// <summary>
        /// Checks if the content of a file on disk matches the newContent
        /// </summary>
        public static bool HasFileContentChanged(string fileName, string newContent)
        {
            if (!File.Exists(fileName))
            {
                return true;
            }

            var oldContent = File.ReadAllText(fileName);

            return oldContent != newContent;
        }
    }
}
