using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using Capgemini.DataMigration.Core.Model;
using CsvHelper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.DataMigration.Core.Tests.Unit.Model
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ObfuscationLookupTests
    {
        [TestMethod]
        public void CountShouldEqualTheNumberOfDatarowsPassedWhenInitialisingTheObject()
        {
            // Arrange
            string name = "testlookup.csv";
            List<dynamic> dataRows = CreateDataRows();

            // Act
            ObfuscationLookup testObject = new ObfuscationLookup(name, dataRows);

            // Assert
            testObject.Count.Should().Be(dataRows.Count);
        }

        [TestMethod]
        public void GetItemShouldReturnTheDataRowAtTheGivenIndex()
        {
            // Arrange
            string name = "testlookup.csv";
            List<dynamic> dataRows = CreateDataRows();

            int index = dataRows.Count - 1;
            string columnName = "postcode";
            var comparisonRecord = dataRows[index];

            ExpandoObject row = dataRows[index];
            var properties = (IDictionary<string, object>)row;
            var comparisonValue = properties[columnName];

            // Act
            ObfuscationLookup testObject = new ObfuscationLookup(name, dataRows);

            // Assert
            testObject[index, columnName].Should().Be(comparisonValue);

        }

        [TestMethod]
        public void GetItemShouldReturnNullIfNoItemWithAKeyOfColumnNameCanBeFound()
        {
            // Arrange
            string name = "testlookup.csv";
            List<dynamic> dataRows = CreateDataRows();

            int index = dataRows.Count - 1;
            string columnName = "NonExistingColumnName";
            var comparisonRecord = dataRows[index];

            // Act
            ObfuscationLookup testObject = new ObfuscationLookup(name, dataRows);

            // Assert
            testObject[index, columnName].Should().BeNull();
        }

        [TestMethod]
        public void GetNameShouldReturnTheNameOfTheLookupFile()
        {
            // Arrange
            string name = "testlookup.csv";
            List<dynamic> dataRows = CreateDataRows();

            // Act
            ObfuscationLookup testObject = new ObfuscationLookup(name, dataRows);

            // Assert
            testObject.Name.Should().Be(name);
        }

        [TestMethod]
        public void PassingEmptyStringAsTheNameParamShouldThrowInvalidArgumentException()
        {
            // Arrange
            List<dynamic> dataRows = CreateDataRows();
            ObfuscationLookup a = null;

            // Act
            Action action = () => a = new ObfuscationLookup(string.Empty, dataRows);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void PassingEmptyListAsTheDataRowsParamShouldThrowInvalidArgumentException()
        {
            // Arrange
            string name = "testlookup.csv";
            List<dynamic> dataRows = new List<dynamic>();
            ObfuscationLookup a = null;

            // Act
            Action action = () => a = new ObfuscationLookup(name, dataRows);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void PassingNullAsTheNameParamShouldThrowInvalidArgumentException()
        {
            // Arrange
            string name = "testlookup.csv";
            List<dynamic> dataRows = CreateDataRows();
            ObfuscationLookup a = null;

            // Act
            Action action = () => a = new ObfuscationLookup(null, dataRows);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void PassingNullAsTheDataRowsParamShouldThrowInvalidArgumentException()
        {
            // Arrange
            string name = "testlookup.csv";
            ObfuscationLookup a = null;

            // Act
            Action action = () => a = new ObfuscationLookup(name, null);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void PassingValidNameAndDataRowsParamsToObjectInitialiserShouldCreateTheObject()
        {
            // Arrange
            string name = "testlookup.csv";
            List<dynamic> dataRows = CreateDataRows();

            // Act
            ObfuscationLookup testObject = new ObfuscationLookup(name, dataRows);

            // Assert
            testObject.Count.Should().Be(dataRows.Count);
            testObject.Name.Should().Be(name);
        }

        private List<dynamic> CreateDataRows()
        {
            string fileName = Path.Combine(GetTestDataPath(), "ukpostcodes.csv");
            List<dynamic> records = new List<dynamic>();

            using (TextReader tr = File.OpenText(fileName))
            {
                using (var reader = new CsvReader(tr))
                {
                    records = reader.GetRecords<dynamic>().ToList();
                }
            }

            return records;
        }

        private string GetTestDataPath()
        {
            string folderPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            var scenarioPath = Path.Combine(folderPath, "TestData", "LookupFiles");
            return scenarioPath;
        }
    }
}
