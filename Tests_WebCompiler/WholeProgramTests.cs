using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebCompiler;

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
                Assert.DoesNotThrow(() => Program.Main("-h"));
                Assert.AreEqual(0, Program.Main("-h"));
                Assert.IsTrue(tw.ToString().Contains("ExcuboLinux.WebCompiler"));
                Assert.IsTrue(tw.ToString().Contains("Usage"));
            }
            using (var tw = new StringWriter())
            {
                Console.SetOut(tw);
                Assert.DoesNotThrow(() => Program.Main("--help"));
                Assert.AreEqual(0, Program.Main("--help"));
                Assert.IsTrue(tw.ToString().Contains("ExcuboLinux.WebCompiler"));
                Assert.IsTrue(tw.ToString().Contains("Usage"));
            }
        }
        [Test]
        public void UseConfig()
        {
            var temporary_files = new List<string>
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
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
            DeleteTemporaryFiles();
            File.WriteAllText("webcompilerconfiguration.json", DefaultConfigFile);
            Assert.DoesNotThrow(() => Program.Main("../../../TestCases/Scss/site.scss", "-c", "webcompilerconfiguration.json"));
            File.Delete("webcompilerconfiguration.json");
            foreach (var output_file in output_files)
            {
                Assert.IsTrue(File.Exists(output_file), $"Output {output_file} should exist");
            }
            foreach (var non_output_file in non_output_files)
            {
                Assert.IsFalse(File.Exists(non_output_file), $"Non-Output {non_output_file} should NOT exist");
            }
            DeleteTemporaryFiles();
            foreach (var tmp_file in temporary_files)
            {
                Assert.IsTrue(File.Exists(tmp_file), $"Temporary {tmp_file} should exist");
            }
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
        }
        [Test]
        public void RejectOutdatedConfig()
        {
            var originalError = Console.Error;
            var scopedError = new StringWriter();
            Console.SetError(scopedError);
            var temporary_files = new List<string>
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
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
            DeleteTemporaryFiles();
            File.WriteAllText("webcompilerconfiguration.json", OutdatedConfigFile);
            Assert.DoesNotThrow(() => Program.Main("../../../TestCases/Scss/site.scss", "-c", "webcompilerconfiguration.json"));
            Assert.AreNotEqual(0, Program.Main("../../../TestCases/Scss/site.scss", "-c", "webcompilerconfiguration.json"));
            File.Delete("webcompilerconfiguration.json");
            foreach (var output_file in output_files)
            {
                Assert.IsFalse(File.Exists(output_file), $"Output {output_file} should exist");
            }
            foreach (var non_output_file in non_output_files)
            {
                Assert.IsFalse(File.Exists(non_output_file), $"Non-Output {non_output_file} should NOT exist");
            }
            DeleteTemporaryFiles();
            foreach (var tmp_file in temporary_files)
            {
                Assert.IsFalse(File.Exists(tmp_file), $"Temporary {tmp_file} should exist");
            }
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
            Assert.IsTrue(scopedError.ToString().Contains("Error reading configuration from file"));
            Console.SetError(originalError);
        }
        [Test]
        public void UseConfigWithRecursion()
        {
            var temporary_files = new List<string>
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
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
            DeleteTemporaryFiles();
            File.WriteAllText("webcompilerconfiguration.json", DefaultConfigFile);
            Assert.DoesNotThrow(() => Program.Main("../../../TestCases/Scss/sub", "-r", "-c", "webcompilerconfiguration.json"));
            File.Delete("webcompilerconfiguration.json");
            foreach (var output_file in output_files)
            {
                Assert.IsTrue(File.Exists(output_file), $"Output {output_file} should exist");
            }
            foreach (var non_output_file in non_output_files)
            {
                Assert.IsFalse(File.Exists(non_output_file), $"Non-Output {non_output_file} should NOT exist");
            }
            DeleteTemporaryFiles();
            foreach (var tmp_file in temporary_files)
            {
                Assert.IsTrue(File.Exists(tmp_file), $"Temporary {tmp_file} should exist");
            }
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
        }
        [Test]
        public void OutputAtSamePlace()
        {
            var temporary_files = new List<string>
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
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
            DeleteTemporaryFiles();
            Assert.DoesNotThrow(() => Program.Main("../../../TestCases/Js/test.js", "../../../TestCases/MinCss/site.min.css", "-r", "../../../TestCases/Css"));
            foreach (var output_file in output_files)
            {
                Assert.IsTrue(File.Exists(output_file), $"Output {output_file} should exist");
            }
            DeleteTemporaryFiles();
            foreach (var tmp_file in temporary_files)
            {
                Assert.IsTrue(File.Exists(tmp_file), $"Temporary {tmp_file} should exist");
            }
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
        }
        [Test]
        public void OutputAtSamePlaceUnnecessarilySpecified()
        {
            var temporary_files = new List<string>
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
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
            DeleteTemporaryFiles();
            Assert.DoesNotThrow(() => Program.Main("../../../TestCases/Js/test.js", "../../../TestCases/MinCss/site.min.css", "-r", "../../../TestCases/Css", "-o", "../../../TestCases"));
            foreach (var output_file in output_files)
            {
                Assert.IsTrue(File.Exists(output_file), $"Output {output_file} should exist");
            }
            DeleteTemporaryFiles();
            foreach (var tmp_file in temporary_files)
            {
                Assert.IsTrue(File.Exists(tmp_file), $"Temporary {tmp_file} should exist");
            }
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
        }
        [Test]
        public void OutputWithPreservation()
        {
            var temporary_files = new List<string>
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
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
            DeleteTemporaryFiles();
            Assert.DoesNotThrow(() => Program.Main("../../../TestCases/Js/test.js", "../../../TestCases/MinCss/site.min.css", "-r", "../../../TestCases/Css", "-o", "../../../output/path"));
            foreach (var output_file in output_files)
            {
                Assert.IsTrue(File.Exists(output_file), $"Output {output_file} should exist");
            }
            DeleteTemporaryFiles();
            foreach (var tmp_file in temporary_files)
            {
                Assert.IsTrue(File.Exists(tmp_file), $"Temporary {tmp_file} should exist");
            }
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
            Directory.Delete("../../../output/path/Css/sub");
            Directory.Delete("../../../output/path/Js");
            Directory.Delete("../../../output/path/MinCss");
            Directory.Delete("../../../output/path/Css");
            Directory.Delete("../../../output/path");
            Directory.Delete("../../../output");
        }
        [Test]
        public void OutputWithoutPreserve()
        {
            var temporary_files = new List<string>
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
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
            DeleteTemporaryFiles();
            Assert.DoesNotThrow(() => Program.Main("../../../TestCases/Js/test.js", "../../../TestCases/MinCss/site.min.css", "-r", "../../../TestCases/Css", "-o", "../../../output/path/", "-p", "d"));
            DeleteTemporaryFiles();
            foreach (var tmp_file in temporary_files)
            {
                Assert.IsFalse(File.Exists(tmp_file), $"Temporary {tmp_file} should not exist");
            }
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
        }
        [Test]
        public void OutputPathNotFullyUsed()
        {
            var temporary_files = new List<string>
            {
                "Css/site.min.css"
            };
            output_files = new List<string>
            {
                "wwwroot/css/site.min.css"
            };
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
            DeleteTemporaryFiles();
            _ = Directory.CreateDirectory("Css");
            File.Copy("../../../TestCases/Css/site.css", "Css/site.css", overwrite: true);
            Assert.DoesNotThrow(() => Program.Main("Css/site.css", "-o", "wwwroot/css", "-p", "d", "-z", "d"));
            Assert.IsTrue(File.Exists(output_files.Last()), "output needs to exist");
            File.Delete("Css/site.css");
            DeleteTemporaryFiles();
            foreach (var tmp_file in temporary_files)
            {
                Assert.IsFalse(File.Exists(tmp_file), $"Temporary {tmp_file} should not exist");
            }
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
            Directory.Delete("Css");
        }

        [Test]
        public void ScssFilesAndFoldersConfiguredToBeIgnoredAreIgnored()
        {
            var temporary_files = new List<string>
            {
            };
            output_files = new List<string>
            {
                "../../../TestCases/Scss/IgnoreFolder/SubFolder/test.css",
                "../../../TestCases/Scss/site.css",
                "../../../TestCases/Scss/test.css",
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
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
            DeleteTemporaryFiles();

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
            var programArgs = new[] { "-c", "webcompilerconfiguration.json", "-r", "../../../TestCases/Scss" };
            Assert.DoesNotThrow(() => Program.Main(programArgs));
            Assert.AreEqual(0, Program.Main(programArgs));
            File.Delete("webcompilerconfiguration.json");
            foreach (var output_file in output_files)
            {
                Assert.IsTrue(File.Exists(output_file), $"Output {output_file} should exist");
            }
            foreach (var non_output_file in non_output_files)
            {
                Assert.IsFalse(File.Exists(non_output_file), $"Non-Output {non_output_file} should NOT exist");
            }
            DeleteTemporaryFiles();
            foreach (var tmp_file in temporary_files)
            {
                Assert.IsTrue(File.Exists(tmp_file), $"Temporary {tmp_file} should exist");
            }
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
        }
    }
}
