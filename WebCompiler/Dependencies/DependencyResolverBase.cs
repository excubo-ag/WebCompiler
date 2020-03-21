using System.Collections.Generic;
using System.IO;

namespace WebCompiler
{
    /// <summary>
    /// Base class for file dependency resolver
    /// </summary>
    public abstract class DependencyResolverBase
    {
        /// <summary>
        /// Stores all resolved dependencies
        /// </summary>
        protected Dictionary<string, Dependencies> Dependencies { get; private set; }

        /// <summary>
        /// The search patterns to use to determine what files should be used to build the dependency tree
        /// </summary>
        public abstract string[] SearchPatterns
        {
            get;
        }

        /// <summary>
        /// The file extension of files of this type
        /// </summary>
        public abstract string FileExtension
        {
            get;
        }

        /// <summary>
        /// Gets the dependency tree
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Dependencies> GetDependencies(string projectRootPath)
        {
            if (Dependencies == null)
            {
                Dependencies = new Dictionary<string, Dependencies>();

                var files = new List<string>();
                foreach (var pattern in SearchPatterns)
                {
                    files.AddRange(Directory.GetFiles(projectRootPath, pattern, SearchOption.AllDirectories));
                }

                foreach (var path in files)
                {
                    UpdateFileDependencies(path);
                }
            }

            return Dependencies;
        }

        /// <summary>
        /// Updates the dependencies for the given file
        /// </summary>
        public abstract void UpdateFileDependencies(string path);
    }
}
