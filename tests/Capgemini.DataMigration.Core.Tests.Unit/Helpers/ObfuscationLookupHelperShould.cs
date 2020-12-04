using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Capgemini.DataMigration.Core.Helpers;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataMigration.Core.Tests.Base;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.DataMigration.Core.Tests.Unit.Helpers
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ObfuscationLookupHelperShould : UnitTestBase
    {
        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
        }

        [TestMethod]
        public void ClearTheLookupList()
        {
            string folderPath = GetTestDataPath();

            ObfuscationLookupHelper.LoadLookups(folderPath);

            int lookupCount = ObfuscationLookupHelper.ObfuscationLookups.Count;

            ObfuscationLookupHelper.CleanLookups();

            ObfuscationLookupHelper.ObfuscationLookups.Count.Should().BeLessThan(lookupCount);
        }

        [TestMethod]
        public void LoadLookupFromADictionary()
        {
            Dictionary<string, ObfuscationLookup> lookups = new Dictionary<string, ObfuscationLookup>();
            string fileName = Directory.GetFiles(GetTestDataPath(), "*.csv", SearchOption.TopDirectoryOnly).First();

            var result = ObfuscationLookupHelper.ReadFromFile(fileName);

            lookups.Add("testLookup.csv", result);
            int lookupCount = lookups.Count;

            ObfuscationLookupHelper.LoadLookups(lookups);

            ObfuscationLookupHelper.ObfuscationLookups.Count.Should().Be(lookupCount);
        }

        [TestMethod]
        public void LoadLookupsFromAFolderPath()
        {
            string folderPath = GetTestDataPath();
            int fileCount = Directory.GetFiles(folderPath, "*.csv", SearchOption.TopDirectoryOnly).Length;

            ObfuscationLookupHelper.LoadLookups(folderPath);

            ObfuscationLookupHelper.ObfuscationLookups.Count.Should().Be(fileCount);
        }

        [TestMethod]
        public void ThrowAnExceptionIfNoLookupFilesCanBeFound()
        {
            string folderPath = Path.Combine(GetTestDataPath(), "EmptyFolder");

            // Act
            Action action = () => ObfuscationLookupHelper.LoadLookups(folderPath);

            action.Should().Throw<FileNotFoundException>();
        }

        [TestMethod]
        public void ReadFromAFile()
        {
            string fileName = Directory.GetFiles(GetTestDataPath(), "*.csv", SearchOption.TopDirectoryOnly).First();

            var result = ObfuscationLookupHelper.ReadFromFile(fileName);

            result.Should().BeOfType(typeof(ObfuscationLookup));
        }

        private static string GetTestDataPath()
        {
            string folderPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            var scenarioPath = Path.Combine(folderPath, "TestData", "LookupFiles");
            return scenarioPath;
        }
    }
}