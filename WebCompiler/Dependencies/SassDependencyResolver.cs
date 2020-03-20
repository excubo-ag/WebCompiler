using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace WebCompiler
{
    internal class SassDependencyResolver : DependencyResolverBase
    {
        public override string[] SearchPatterns => new[] { "*.scss", "*.sass" };

        public override string FileExtension => ".scss";


        /// <summary>
        /// Updates the dependencies of a single file
        /// </summary>
        /// <param name="path"></param>
        public override void UpdateFileDependencies(string path)
        {
            if (Dependencies != null)
            {
                FileInfo info = new FileInfo(path);
                path = info.FullName;

                if (!Dependencies.ContainsKey(path))
                {
                    Dependencies[path] = new Dependencies();
                }

                //remove the dependencies registration of this file
                Dependencies[path].DependentOn = new HashSet<string>();
                //remove the dependentfile registration of this file for all other files
                foreach (string dependenciesPath in Dependencies.Keys)
                {
                    string lowerDependenciesPath = dependenciesPath;
                    if (Dependencies[lowerDependenciesPath].DependentFiles.Contains(path))
                    {
                        Dependencies[lowerDependenciesPath].DependentFiles.Remove(path);
                    }
                }

                string content = File.ReadAllText(info.FullName);

                //match both <@import "myFile.scss";> and <@import url("myFile.scss");> syntax
                MatchCollection matches = Regex.Matches(content, @"(?<=@import(?:[\s]+))(?:(?:\(\w+\)))?\s*(?:url)?(?<url>[^;]+)", RegexOptions.Multiline);
                foreach (Match match in matches)
                {
                    IEnumerable<FileInfo> importedfiles = GetFileInfos(info, match);

                    foreach (FileInfo importedfile in importedfiles)
                    {
                        if (importedfile == null)
                        {
                            continue;
                        }

                        FileInfo theFile = importedfile;

                        //if the file doesn't end with the correct extension, an import statement without extension is probably used, to re-add the extension (#175)
                        if (string.Compare(importedfile.Extension, FileExtension, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            theFile = new FileInfo(importedfile.FullName + FileExtension);
                        }

                        string dependencyFilePath = theFile.FullName;

                        if (!File.Exists(dependencyFilePath))
                        {
                            // Trim leading underscore to support Sass partials
                            string dir = Path.GetDirectoryName(dependencyFilePath);
                            string fileName = Path.GetFileName(dependencyFilePath);
                            string cleanPath = Path.Combine(dir, "_" + fileName);

                            if (!File.Exists(cleanPath))
                            {
                                continue;
                            }

                            dependencyFilePath = cleanPath;
                        }

                        if (!Dependencies[path].DependentOn.Contains(dependencyFilePath))
                        {
                            Dependencies[path].DependentOn.Add(dependencyFilePath);
                        }

                        if (!Dependencies.ContainsKey(dependencyFilePath))
                        {
                            Dependencies[dependencyFilePath] = new Dependencies();
                        }

                        if (!Dependencies[dependencyFilePath].DependentFiles.Contains(path))
                        {
                            Dependencies[dependencyFilePath].DependentFiles.Add(path);
                        }
                    }
                }
            }
        }

        private static IEnumerable<FileInfo> GetFileInfos(FileInfo info, System.Text.RegularExpressions.Match match)
        {
            string url = match.Groups["url"].Value.Replace("'", "\"").Replace("(", "").Replace(")", "").Replace(";", "").Trim();
            List<FileInfo> list = new List<FileInfo>();

            foreach (string name in url.Split(new[] { "\"," }, StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    string value = name.Replace("\"", "").Replace('/', Path.DirectorySeparatorChar).Trim();
                    list.Add(new FileInfo(Path.Combine(info.DirectoryName, value)));
                }
                catch (Exception ex)
                {
                    // Not a valid file name
                    System.Diagnostics.Debug.Write(ex);
                }
            }

            return list;
        }
    }
}
