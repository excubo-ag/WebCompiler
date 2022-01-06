using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebCompiler.Compile;
using WebCompiler.Configuration;

namespace Tests_WebCompiler
{
    public class ScssIgnoreFileFolderTests
    {
        [Test]
        public void Test()
        {
            var basePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var folderToRecurse =  "../../../TestCases/Scss";
            var foldersToIgnore = new List<string>() { "../../../TestCases/Scss/IgnoreFolder" };
            var filesToIgnore = new List<string>() { "../../../TestCases/Scss/error.scss" };

            var config = new Config();
            config.CompilerSettings.IgnoreFolders = foldersToIgnore;
            config.CompilerSettings.IgnoreFiles = filesToIgnore;

            var compiler = new Compilers(config, basePath);

            foreach (var file in WebCompiler.Helpers.FileFolderHelpers.Recurse(folderToRecurse))
            {
                var result = compiler.TryCompile(file);
                Assert.IsTrue(result.Errors == null || !result.Errors.Any());
            }

            config.CompilerSettings.IgnoreFolders.Clear();
            config.CompilerSettings.IgnoreFiles.Clear();
            var errorsEncountered = false;
            foreach (var file in WebCompiler.Helpers.FileFolderHelpers.Recurse(basePath))
            {
                var result = compiler.TryCompile(file);
                if(result.Errors != null) errorsEncountered = true;
            }
            Assert.IsTrue(errorsEncountered);
        }
    }
}