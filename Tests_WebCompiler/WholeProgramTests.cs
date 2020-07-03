using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using WebCompiler;

namespace Tests_WebCompiler
{
    public class WholeProgramTests : TestsBase
    {
        [Test]
        public void ShowHelp()
        {
            using (var tw = new StringWriter())
            {
                Console.SetOut(tw);
                Assert.DoesNotThrow(() => Program.Main("-h"));
                Assert.AreEqual(0, Program.Main("-h"));
                Assert.IsTrue(tw.ToString().Contains("Excubo.WebCompiler"));
                Assert.IsTrue(tw.ToString().Contains("Usage"));
            }
            using (var tw = new StringWriter())
            {
                Console.SetOut(tw);
                Assert.DoesNotThrow(() => Program.Main("--help"));
                Assert.AreEqual(0, Program.Main("--help"));
                Assert.IsTrue(tw.ToString().Contains("Excubo.WebCompiler"));
                Assert.IsTrue(tw.ToString().Contains("Usage"));
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
            Assert.DoesNotThrow(() => Program.Main("../../../TestCases/Js", "../../../TestCases/MinCss/site.min.css", "-r", "../../../TestCases/Css"));
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
            Assert.DoesNotThrow(() => Program.Main("../../../TestCases/Js", "../../../TestCases/MinCss/site.min.css", "-r", "../../../TestCases/Css", "-o", "../../../TestCases"));
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
                "../../../output/Js/test.min.js.gz",
                "../../../output/MinCss/site.min.css.gz",
                "../../../output/Css/site.min.css.gz",
                "../../../output/Css/test.min.css.gz",
                "../../../output/Css/sub/site.min.css.gz"
            };
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
            DeleteTemporaryFiles();
            Assert.DoesNotThrow(() => Program.Main("../../../TestCases/Js", "../../../TestCases/MinCss/site.min.css", "-r", "../../../TestCases/Css", "-o", "../../../output"));
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
                "../../../output/Js/test.min.js.gz",
                "../../../output/MinCss/site.min.css.gz",
                "../../../output/Css/site.min.css.gz",
                "../../../output/Css/test.min.css.gz",
                "../../../output/Css/sub/site.min.css.gz"
            };
            foreach (var tmp_file in temporary_files)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
            }
            DeleteTemporaryFiles();
            Assert.DoesNotThrow(() => Program.Main("../../../TestCases/Js", "../../../TestCases/MinCss/site.min.css", "-r", "../../../TestCases/Css", "-o", "../../../output", "-p", "d"));
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
    }
}
