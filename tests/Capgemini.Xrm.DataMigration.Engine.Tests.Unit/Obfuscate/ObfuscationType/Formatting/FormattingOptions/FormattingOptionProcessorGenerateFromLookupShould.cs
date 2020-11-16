using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting.FormattingOptions;
using CsvHelper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.Obfuscate.ObfuscationType.Formatting.FormattingOptions
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class FormattingOptionProcessorGenerateFromLookupShould : UnitTestBase
    {
        private FormattingOptionProcessor systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();

            ObfuscationLookup lookups = new ObfuscationLookup("testlookup.csv", CreateDataRows());

            var mockLookup = new Mock<FormattingOptionLookup>();
            mockLookup.Setup(a => a.GetObfuscationLookup(It.IsAny<string>()))
                .Returns(lookups);

            systemUnderTest = new FormattingOptionProcessor(mockLookup.Object);
        }

        [TestMethod]
        public void ReturnAValueFromALookupThatIsDifferentToTheOriginalValue()
        {
            var originalValue = "Tester";

            var newValue = systemUnderTest.GenerateFromLookup(originalValue, GenerateValidArgs());

            newValue.Should().NotBe(originalValue);
        }

        [TestMethod]
        public void ThrowArgumentNullExceptionIfArgsParamIsNull()
        {
            var originalValue = "Tester";

            Action action = () => systemUnderTest.GenerateFromLookup(originalValue, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ThrowArgumentNullExceptionIfOriginalValueIsNull()
        {
            Action action = () => systemUnderTest.GenerateFromLookup(null, GenerateValidArgs());

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ThrowValidationExceptionIfFilenameIsNotPassedAsAnArgument()
        {
            var originalValue = "Tester";
            var args = new Dictionary<string, string>();
            args.Add("columnname", "column");
            var arg = new ObfuscationFormatOption(ObfuscationFormatType.Lookup, args);

            Action action = () => systemUnderTest.GenerateFromLookup(originalValue, arg);

            action.Should().Throw<Capgemini.DataMigration.Exceptions.ValidationException>();
        }

        [TestMethod]
        public void ThrowValidationExceptionIfColumnNameIsNotPassedAsAnArgument()
        {
            var originalValue = "Tester";
            var args = new Dictionary<string, string>
            {
                { "filename", "testlookup.csv" }
            };
            var arg = new ObfuscationFormatOption(ObfuscationFormatType.Lookup, args);

            Action action = () => systemUnderTest.GenerateFromLookup(originalValue, arg);

            action.Should()
                  .Throw<Capgemini.DataMigration.Exceptions.ValidationException>()
                  .WithMessage("Obfuscation format option columnName should not be null");
        }

        private static ObfuscationFormatOption GenerateValidArgs()
        {
            var args = new Dictionary<string, string>();
            args.Add("filename", "testlookup.csv");
            args.Add("columnname", "postcode");
            var arg = new ObfuscationFormatOption(ObfuscationFormatType.Lookup, args);
            return arg;
        }

        private static string GetTestDataPath()
        {
            string folderPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            var scenarioPath = Path.Combine(folderPath, "TestData", "LookupFiles");
            return scenarioPath;
        }

        private List<dynamic> CreateDataRows()
        {
            string fileName = Path.Combine(GetTestDataPath(), "ukpostcodes.csv");
            List<dynamic> records = null;

            using (TextReader tr = File.OpenText(fileName))
            {
                using (var reader = new CsvReader(tr))
                {
                    records = reader.GetRecords<dynamic>().ToList();
                }
            }

            return records;
        }
    }
}