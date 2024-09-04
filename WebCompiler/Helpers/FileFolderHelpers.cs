using Microsoft.Extensions.FileSystemGlobbing;
using System.Collections.Generic;

namespace WebCompiler.Helpers
{
	public static class FileFolderHelpers
	{
		public static IEnumerable<string> RecurseRespectingExclusions(string directory, List<string> ignoreGlobs)
		{
			var matcher = new Matcher();
			matcher.AddInclude("**/*");
			matcher.AddExcludePatterns(ignoreGlobs);
			foreach (string file in matcher.GetResultsInFullPath(directory))
			{
				yield return file;
			}
		}
		public static bool TestFileForExclusion(string directory, List<string> ignoreGlobs, string file)
		{
			var matcher = new Matcher();
			matcher.AddInclude("**/*");
			matcher.AddExcludePatterns(ignoreGlobs);
			return matcher.Match(directory, file).HasMatches;
		}
	}
}