using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebCompiler;
using WebCompiler.Helpers;

namespace Tests_WebCompiler
{
    public class WholeProgramTests : TestsBase
	{
		private const string DefaultConfigFile = @"{
  ""Minifiers"": {
    ""GZip"": false,
    ""Enabled"": false
  },
  ""CompilerSettings"": {
    ""Sass"": {
      ""IndentType"": ""Space"",
      ""IndentWidth"": 2,
      ""OutputStyle"": ""Expanded"",
      ""RelativeUrls"": true,
      ""LineFeed"": ""Lf"",
      ""SourceMap"": true
    }
  },
  ""Output"": {
    ""Preserve"": true
  }
}";
		private const string ConfigFileIgnoringSiteScss = @"{
  ""Minifiers"": {
    ""GZip"": false,
    ""Enabled"": false
  },
  ""CompilerSettings"": {
    // Ignore a file
    ""Ignore"": [ 
        ""**/_*.*"", 
        ""site.scss"" /* This file would normally be handled */
    ],
    ""Sass"": {
      ""IndentType"": ""Space"",
      ""IndentWidth"": 2,
      ""OutputStyle"": ""Expanded"",
      ""RelativeUrls"": true,
      ""LineFeed"": ""Lf"",
      ""SourceMap"": true
    }
  },
  ""Output"": {
    ""Preserve"": true
  }
}";
		private const string OutdatedConfigFile = @"{
  ""Minifiers"": {
    ""GZip"": false,
    ""Enabled"": false
  },
  ""CompilerSettings"": {
    ""Sass"": {
      ""IndentType"": ""Space"",
      ""IndentWidth"": 2,
      ""OutputStyle"": ""Nested"",
      ""Precision"": 5,
      ""RelativeUrls"": true,
      ""LineFeed"": ""Lf"",
      ""SourceMap"": true
    }
  },
  ""Output"": {
    ""Preserve"": true
  }
}";
        [Test]
        public void ShowHelp()
        {
            using (var tw = new StringWriter())
            {
                Console.SetOut(tw);
                Assert.That(Program.Main("-h"), Is.EqualTo(0));
                Assert.That(tw.ToString().Contains("Excubo.WebCompiler"), Is.True);
                Assert.That(tw.ToString().Contains("Usage"), Is.True);
            }
            using (var tw = new StringWriter())
            {
                Console.SetOut(tw);
                Assert.That(Program.Main("--help"), Is.EqualTo(0));
                Assert.That(tw.ToString().Contains("Excubo.WebCompiler"), Is.True);
                Assert.That(tw.ToString().Contains("Usage"), Is.True);
            }
		}
		[Test]
		public void UseConfig()
		{
			temporary_files = new List<string>
			{
			};
			output_files = new List<string>
			{
				"../../../TestCases/Scss/site.css"
			};
			var non_output_files = new List<string> // supressed by config: no gzip and no minification
            {
				"../../../TestCases/Scss/site.min.css",
				"../../../TestCases/Scss/site.min.css.gz"
			};
            DeleteTemporaryFiles();
			DeleteOutputFiles();
			File.WriteAllText("webcompilerconfiguration.json", DefaultConfigFile);
            input = "../../../TestCases/Scss/site.scss";
            Assert.That(Program.Main("../../../TestCases/Scss/site.scss", "-c", "webcompilerconfiguration.json"), Is.EqualTo(0));
			File.Delete("webcompilerconfiguration.json");
			foreach (var output_file in output_files)
			{
				Assert.That(File.Exists(output_file), Is.True, $"Output {output_file} should exist");
			}
			foreach (var non_output_file in non_output_files)
			{
				Assert.That(File.Exists(non_output_file), Is.False, $"Non-Output {non_output_file} should NOT exist");
			}
			DeleteOutputFiles();
			foreach (var tmp_file in temporary_files)
			{
				Assert.That(File.Exists(tmp_file), Is.True, $"Temporary {tmp_file} should exist");
            }
		}
		[Test]
		public void UseConfigIgnoreFile()
		{
		    temporary_files = new List<string>
			{
			};
			output_files = new List<string>
			{
			};
			var non_output_files = new List<string>
            {
				"../../../TestCases/Scss/site.css", // ignored through config ignore
				"../../../TestCases/Scss/site.min.css",
				"../../../TestCases/Scss/site.min.css.gz"
			};
            DeleteTemporaryFiles();
            DeleteOutputFiles();
			File.WriteAllText("webcompilerconfiguration.json", ConfigFileIgnoringSiteScss);
			Assert.That(Program.Main("../../../TestCases/Scss/site.scss", "-c", "webcompilerconfiguration.json"), Is.EqualTo(0));
			File.Delete("webcompilerconfiguration.json");
			foreach (var output_file in output_files)
			{
				Assert.That(File.Exists(output_file), Is.True, $"Output {output_file} should exist");
			}
			foreach (var non_output_file in non_output_files)
			{
				Assert.That(File.Exists(non_output_file), Is.False, $"Non-Output {non_output_file} should NOT exist");
			}
			DeleteOutputFiles();
			foreach (var tmp_file in temporary_files)
			{
				Assert.That(File.Exists(tmp_file), Is.True, $"Temporary {tmp_file} should exist");
            }
        }
		[Test]
		public void UseConfigWithOverride()
		{
			temporary_files = new List<string>
			{
			};
            // supressed by config: no gzip and no minification, but then overriden by flags
			output_files = new List<string>
			{
				"../../../TestCases/Scss/site.css",
				"../../../TestCases/Scss/site.min.css",
				"../../../TestCases/Scss/site.min.css.gz"
			};
            DeleteTemporaryFiles();
            DeleteOutputFiles();
			File.WriteAllText("webcompilerconfiguration.json", DefaultConfigFile);
            input = "../../../TestCases/Scss/site.scss";
            Assert.That(Program.Main("../../../TestCases/Scss/site.scss", "-c", "webcompilerconfiguration.json", "-m", "enable", "-z", "enable"), Is.EqualTo(0));
			File.Delete("webcompilerconfiguration.json");
			foreach (var output_file in output_files)
			{
				Assert.That(File.Exists(output_file), Is.True, $"Output {output_file} should exist");
			}
			DeleteOutputFiles();
			foreach (var tmp_file in temporary_files)
			{
				Assert.That(File.Exists(tmp_file), Is.True, $"Temporary {tmp_file} should exist");
            }
        }
		[Test]
        public void RejectOutdatedConfig()
        {
            var originalError = Console.Error;
            var scopedError = new StringWriter();
            Console.SetError(scopedError);
            temporary_files = new List<string>
            {
            };
            output_files = new List<string>
            {
                "../../../TestCases/Scss/site.css"
            };
            var non_output_files = new List<string> // supressed by config: no gzip and no minification
            {
                "../../../TestCases/Scss/site.min.css",
                "../../../TestCases/Scss/site.min.css.gz"
            };
            DeleteTemporaryFiles();
            DeleteOutputFiles();
            File.WriteAllText("webcompilerconfiguration.json", OutdatedConfigFile);
            input = "../../../TestCases/Scss/site.scss";
            Assert.That(Program.Main("../../../TestCases/Scss/site.scss", "-c", "webcompilerconfiguration.json"), Is.Not.EqualTo(0));
            File.Delete("webcompilerconfiguration.json");
            foreach (var output_file in output_files)
            {
                Assert.That(File.Exists(output_file), Is.False, $"Output {output_file} should exist");
            }
            foreach (var non_output_file in non_output_files)
            {
                Assert.That(File.Exists(non_output_file), Is.False, $"Non-Output {non_output_file} should NOT exist");
            }
            DeleteOutputFiles();
            foreach (var tmp_file in temporary_files)
            {
                Assert.That(File.Exists(tmp_file), Is.False, $"Temporary {tmp_file} should exist");
            }
            DeleteTemporaryFiles();
            Assert.That(scopedError.ToString().Contains("Error reading configuration from file"), Is.True);
            Console.SetError(originalError);
        }
        [Test]
        public void UseConfigWithRecursion()
        {
            temporary_files = new List<string>
            {
            };
            output_files = new List<string>
            {
                "../../../TestCases/Scss/sub/foo.css",
                "../../../TestCases/Scss/sub/relative.css"
            };
            var non_output_files = new List<string> // supressed by config: no gzip and no minification
            {
                "../../../TestCases/Scss/sub/foo.min.css",
                "../../../TestCases/Scss/sub/foo.min.css.gz",
                "../../../TestCases/Scss/sub/relative.min.css",
                "../../../TestCases/Scss/sub/relative.min.css.gz"
            };
            DeleteTemporaryFiles();
            DeleteOutputFiles();
            File.WriteAllText("webcompilerconfiguration.json", DefaultConfigFile);
            input_files = FileFolderHelpers.RecurseRespectingExclusions("../../../TestCases/Scss/sub", new List<string>()).ToList();
            Assert.That(Program.Main("../../../TestCases/Scss/sub", "-r", "-c", "webcompilerconfiguration.json"), Is.EqualTo(0));
            File.Delete("webcompilerconfiguration.json");
            foreach (var output_file in output_files)
            {
                Assert.That(File.Exists(output_file), Is.True, $"Output {output_file} should exist");
            }
            foreach (var non_output_file in non_output_files)
            {
                Assert.That(File.Exists(non_output_file), Is.False, $"Non-Output {non_output_file} should NOT exist");
            }
            DeleteOutputFiles();
            foreach (var tmp_file in temporary_files)
            {
                Assert.That(File.Exists(tmp_file), $"Temporary {tmp_file} should exist");
            }
        }
        [Test]
        public void OutputAtSamePlace()
        {
            temporary_files = new List<string>
            {
                "../../../TestCases/Css/site.min.css",
                "../../../TestCases/Css/test.min.css",
                "../../../TestCases/Css/sub/site.min.css",
                "../../../TestCases/Js/test.min.js"
            };
            output_files = new List<string>
            {
                "../../../TestCases/Js/test.min.js.gz",
                "../../../TestCases/MinCss/site.min.css.gz",
                "../../../TestCases/Css/site.min.css.gz",
                "../../../TestCases/Css/test.min.css.gz",
                "../../../TestCases/Css/sub/site.min.css.gz"
            };
            DeleteTemporaryFiles();
            DeleteOutputFiles();
            input_files = FileFolderHelpers.RecurseRespectingExclusions("../../../TestCases/Css", new List<string>()).Append("../../../TestCases/Js/test.js").Append("../../../TestCases/MinCss/site.min.css").ToList();
            Assert.That(Program.Main("../../../TestCases/Js/test.js", "../../../TestCases/MinCss/site.min.css", "-r", "../../../TestCases/Css", "-z", "enable"), Is.EqualTo(0));
            foreach (var output_file in output_files)
            {
                Assert.That(File.Exists(output_file), $"Output {output_file} should exist");
            }
            DeleteOutputFiles();
            foreach (var tmp_file in temporary_files)
            {
                Assert.That(File.Exists(tmp_file), $"Temporary {tmp_file} should exist");
            }
        }
        [Test]
        public void OutputAtSamePlaceUnnecessarilySpecified()
        {
            temporary_files = new List<string>
            {
                "../../../TestCases/Css/site.min.css",
                "../../../TestCases/Css/test.min.css",
                "../../../TestCases/Css/sub/site.min.css",
                "../../../TestCases/Js/test.min.js"
            };
            output_files = new List<string>
            {
                "../../../TestCases/Js/test.min.js.gz",
                "../../../TestCases/MinCss/site.min.css.gz",
                "../../../TestCases/Css/site.min.css.gz",
                "../../../TestCases/Css/test.min.css.gz",
                "../../../TestCases/Css/sub/site.min.css.gz"
            };
            DeleteTemporaryFiles();
            DeleteOutputFiles();
            input_files = FileFolderHelpers.RecurseRespectingExclusions("../../../TestCases/Css", new List<string>()).Append("../../../TestCases/Js/test.js").Append("../../../TestCases/MinCss/site.min.css").ToList();
            Assert.That(Program.Main("../../../TestCases/Js/test.js", "../../../TestCases/MinCss/site.min.css", "-r", "../../../TestCases/Css", "-o", "../../../TestCases", "-z", "enable"), Is.EqualTo(0));
            foreach (var output_file in output_files)
            {
                Assert.That(File.Exists(output_file), $"Output {output_file} should exist");
            }
            DeleteOutputFiles();
            foreach (var tmp_file in temporary_files)
            {
                Assert.That(File.Exists(tmp_file), $"Temporary {tmp_file} should exist");
            }
        }
        [Test]
        public void OutputWithPreservation()
        {
            temporary_files = new List<string>
            {
                "../../../TestCases/Css/site.min.css",
                "../../../TestCases/Css/site.min.css.gz",
                "../../../TestCases/Css/test.min.css",
                "../../../TestCases/Css/test.min.css.gz",
                "../../../TestCases/Css/sub/site.min.css",
                "../../../TestCases/Css/sub/site.min.css.gz",
                "../../../TestCases/Js/test.min.js",
                "../../../TestCases/Js/test.min.js.gz",
                "../../../TestCases/MinCss/site.min.css.gz",
            };
            output_files = new List<string>
            {
                "../../../output/path/Js/test.min.js.gz",
                "../../../output/path/MinCss/site.min.css.gz",
                "../../../output/path/Css/site.min.css.gz",
                "../../../output/path/Css/test.min.css.gz",
                "../../../output/path/Css/sub/site.min.css.gz"
            };
            DeleteTemporaryFiles();
            DeleteOutputFiles();
            input_files = FileFolderHelpers.RecurseRespectingExclusions("../../../TestCases/Css", new List<string>()).Append("../../../TestCases/Js/test.js").Append("../../../TestCases/MinCss/site.min.css").ToList();
            Assert.That(Program.Main("../../../TestCases/Js/test.js", "../../../TestCases/MinCss/site.min.css", "-r", "../../../TestCases/Css", "-o", "../../../output/path", "-z", "enable"), Is.EqualTo(0));
            foreach (var output_file in output_files)
            {
                Assert.That(File.Exists(output_file), $"Output {output_file} should exist");
            }
            DeleteOutputFiles();
            foreach (var tmp_file in temporary_files)
            {
                Assert.That(File.Exists(tmp_file), $"Temporary {tmp_file} should exist");
            }
        }

        [Test]
        public void OutputWithoutPreserve()
        {
            temporary_files = new List<string>
            {
                "../../../TestCases/Css/site.min.css",
                "../../../TestCases/Css/site.min.css.gz",
                "../../../TestCases/Css/test.min.css",
                "../../../TestCases/Css/test.min.css.gz",
                "../../../TestCases/Css/sub/site.min.css",
                "../../../TestCases/Css/sub/site.min.css.gz",
                "../../../TestCases/Js/test.min.js",
                "../../../TestCases/Css/test.min.js.gz",
                "../../../TestCases/MinCss/site.min.js.gz",
            };
            output_files = new List<string>
            {
                "../../../output/path/Js/test.min.js.gz",
                "../../../output/path/MinCss/site.min.css.gz",
                "../../../output/path/Css/site.min.css.gz",
                "../../../output/path/Css/test.min.css.gz",
                "../../../output/path/Css/sub/site.min.css.gz"
            };
            DeleteTemporaryFiles();
            DeleteOutputFiles();
            input_files = FileFolderHelpers.RecurseRespectingExclusions("../../../TestCases/Css", new List<string>()).Append("../../../TestCases/Js/test.js").Append("../../../TestCases/MinCss/site.min.css").ToList();
            Assert.That(Program.Main("../../../TestCases/Js/test.js", "../../../TestCases/MinCss/site.min.css", "-r", "../../../TestCases/Css", "-o", "../../../output/path/", "-p", "d", "-z", "enable"), Is.EqualTo(0));
            DeleteOutputFiles();
            foreach (var tmp_file in temporary_files)
            {
                Assert.That(File.Exists(tmp_file), Is.False, $"Temporary {tmp_file} should not exist");
            }
        }
        [Test]
        public void OutputPathNotFullyUsed()
        {
            temporary_files = new List<string>
            {
                "Css/site.min.css"
            };
            output_files = new List<string>
            {
                "wwwroot/css/site.min.css"
            };
            DeleteTemporaryFiles();
            DeleteOutputFiles();
            _ = Directory.CreateDirectory("Css");
            File.Copy("../../../TestCases/Css/site.css", "Css/site.css", overwrite: true);
            input = "Css/site.css";
            Assert.That(Program.Main("Css/site.css", "-o", "wwwroot/css", "-p", "d", "-z", "d"), Is.EqualTo(0));
            Assert.That(File.Exists(output_files.Last()), "output needs to exist");
            File.Delete("Css/site.css");
            DeleteOutputFiles();
            foreach (var tmp_file in temporary_files)
            {
                Assert.That(File.Exists(tmp_file), Is.False, $"Temporary {tmp_file} should not exist");
            }
        }

        [Test]
        public void ScssFilesAndFoldersConfiguredToBeIgnoredAreIgnored()
        {
            temporary_files = new List<string>
            {
            };
            output_files = new List<string>
            {
                "../../../TestCases/Scss/IgnoreFolder/SubFolder/test.css",
                "../../../TestCases/Scss/_variables.css",
                "../../../TestCases/Scss/site.css",
                "../../../TestCases/Scss/dependency.css",
                "../../../TestCases/Scss/use_with_override.css",
                "../../../TestCases/Scss/test.css",
                "../../../TestCases/Scss/sub/_bar.css",
                "../../../TestCases/Scss/sub/foo.css",
                "../../../TestCases/Scss/sub/relative.css"
            };
            var non_output_files = new List<string>
            {
                // suppressed by Ignore Globs
                "../../../TestCases/Scss/IgnoreFolder/globalVariables.css",
                "../../../TestCases/Scss/IgnoreFolder2/globalVariables.css",
                "../../../TestCases/Scss/IgnoreFolder2/SubFolder/test.css",
                "../../../TestCases/Scss/IgnoreFolderAndSubFolders/globalVariables.css",
                "../../../TestCases/Scss/IgnoreFolderAndSubFolders/SubFolder1/test.css",
                "../../../TestCases/Scss/IgnoreFolderAndSubFolders/SubFolder2/test.css",
                "../../../TestCases/Scss/error.css",
                "../../../TestCases/Scss/globalVariables.css",
                // supressed by config: no gzip and no minification
                "../../../TestCases/Scss/site.min.css",
                "../../../TestCases/Scss/site.min.css.gz"
            };
            DeleteTemporaryFiles();
            DeleteOutputFiles();

            var config = @"{
  ""Minifiers"": {
    ""GZip"": false,
    ""Enabled"": false
  },
  ""CompilerSettings"": {
    ""Ignore"": [
        ""IgnoreFolder/*.scss"",
        ""IgnoreFolder2/**/*"",
        ""IgnoreFolderAndSubFolders/**/*.scss"",
        ""error.scss"",
        ""globalVar*.scss""
    ],
    ""Sass"": {
      ""IndentType"": ""Space"",
      ""IndentWidth"": 2,
      ""OutputStyle"": ""Expanded"",
      ""RelativeUrls"": true,
      ""LineFeed"": ""Lf"",
      ""SourceMap"": true
    }
  },
  ""Output"": {
    ""Preserve"": true
  }
}";
            File.WriteAllText("webcompilerconfiguration.json", config);
            input_files = FileFolderHelpers.RecurseRespectingExclusions("../../../TestCases/Scss", new List<string>
            {
                "IgnoreFolder/*.scss",
                "IgnoreFolder2/**/*",
                "IgnoreFolderAndSubFolders/**/*.scss",
                "error.scss",
                "globalVar*.scss"
            }).ToList();
            var programArgs = new[] { "-c", "webcompilerconfiguration.json", "-r", "../../../TestCases/Scss" };
            Assert.That(Program.Main(programArgs), Is.EqualTo(0));
            File.Delete("webcompilerconfiguration.json");
            foreach (var output_file in output_files)
            {
                Assert.That(File.Exists(output_file), $"Output {output_file} should exist");
            }
            foreach (var non_output_file in non_output_files)
            {
                Assert.That(File.Exists(non_output_file), Is.False, $"Non-Output {non_output_file} should NOT exist");
            }
            DeleteOutputFiles();
            foreach (var tmp_file in temporary_files)
            {
                Assert.That(File.Exists(tmp_file), $"Temporary {tmp_file} should exist");
            }
        }
    }
}
