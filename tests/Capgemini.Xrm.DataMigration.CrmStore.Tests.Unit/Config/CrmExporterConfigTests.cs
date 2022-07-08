using System.IO;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Capgemini.Xrm.DataMigration.CrmStore.Config.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmExporterConfigTests : UnitTestBase
    {
        private CrmExporterConfig systemUnderTest;
        private Mock<IEntityMetadataCache> mockEntityMetadataCache;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
            systemUnderTest = new CrmExporterConfig();
            mockEntityMetadataCache = new Mock<IEntityMetadataCache>();
        }

        [TestMethod]
        public void GetFetchXMLQueriesWithoutSettingConfigProperties()
        {
            var actual = systemUnderTest.GetFetchXMLQueries(mockEntityMetadataCache.Object);

            actual.Count.Should().Be(0);
        }

        [TestMethod]
        public void GetFetchXMLQueriesWithValueForFetchXMLFolderPath()
        {
            systemUnderTest.FetchXMLFolderPath = "TestData";

            var actual = systemUnderTest.GetFetchXMLQueries(mockEntityMetadataCache.Object);

            actual.Count.Should().Be(1);
        }

        [TestMethod]
        public void GetFetchXMLQueriesWithValueForFetchXMLFolderPathAndCrmMigrationToolSchemaPaths()
        {
            systemUnderTest.FetchXMLFolderPath = "TestData";
            systemUnderTest.CrmMigrationToolSchemaPaths.Add("TestData\\usersettingsschema.xml");

            var actual = systemUnderTest.GetFetchXMLQueries(mockEntityMetadataCache.Object);

            actual.Count.Should().BeGreaterThan(2);
        }

        [TestMethod]
        public void SaveConfiguration()
        {
            var filePath = "TestData/TestExportConfig.json";
            SafelyDeleteFile(filePath);

            FluentActions.Invoking(() => systemUnderTest.SaveConfiguration(filePath))
                       .Should()
                       .NotThrow();

            Assert.IsTrue(File.Exists(filePath));
        }

        [TestMethod]
        public void GetConfiguration()
        {
            var filePath = "TestData/ExportConfig.json";

            var actual = CrmExporterConfig.GetConfiguration(filePath);

            actual.PageSize.Should().Be(500);
            actual.BatchSize.Should().Be(500);
            actual.TopCount.Should().Be(100000);
            actual.OnlyActiveRecords.Should().Be(false);
            actual.JsonFolderPath.Should().EndWith("TestData");
            actual.OneEntityPerBatch.Should().Be(true);
            actual.FilePrefix.Should().Be("ExportedData");
            actual.SeperateFilesPerEntity.Should().Be(true);
        }
    }
}