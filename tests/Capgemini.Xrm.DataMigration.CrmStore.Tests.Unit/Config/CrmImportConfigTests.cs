using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Config;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Capgemini.Xrm.DataMigration.CrmStore.Config.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmImportConfigTests : UnitTestBase
    {
        private CrmImportConfig systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
            systemUnderTest = new CrmImportConfig();
        }

        [TestMethod]
        public void Instantiated()
        {
            systemUnderTest.PassOneReferences.Count.Should().Be(4);
            systemUnderTest.FilePrefix.Should().Be("ExportedData");
            systemUnderTest.ProcessesToDeactivate.Should().NotBeNull();
            systemUnderTest.PluginsToDeactivate.Should().NotBeNull();
            systemUnderTest.NoUpsertEntities.Should().NotBeNull();
            systemUnderTest.IgnoreStatusesExceptions.Should().NotBeNull();
            systemUnderTest.AdditionalFieldsToIgnore.Should().NotBeNull();
            systemUnderTest.EntitiesToSync.Should().NotBeNull();
            systemUnderTest.NoUpdateEntities.Should().NotBeNull();
        }

        [TestMethod]
        public void GetConfiguration()
        {
            var filePath = "TestData/ImportConfig.json";

            var actual = CrmImportConfig.GetConfiguration(filePath);

            actual.FilePrefix.Should().Be("ExportedData");
        }

        [TestMethod]
        public void SaveConfiguration()
        {
            var filePath = "TestData/TestImportConfig.json";
            SafelyDeleteFile(filePath);

            FluentActions.Invoking(() => systemUnderTest.SaveConfiguration(filePath))
                       .Should()
                       .NotThrow();

            Assert.IsTrue(File.Exists(filePath));
        }

        [TestMethod]
        public void SerializeFromPostDeployDataImport()
        {
            var content = File.ReadAllText("TestData/PostDeployDataImport.json");
            var settings = JsonSerializerConfig.SerializerSettings;

            var actual = JsonConvert.DeserializeObject<CrmImportConfig>(content, settings);

            actual.PassOneReferences.Count.Should().Be(9);
            actual.PassOneReferences.SingleOrDefault(x => x == "businessunit").Should().NotBeNullOrEmpty();
            actual.PassOneReferences.SingleOrDefault(x => x == "uom").Should().NotBeNullOrEmpty();
            actual.PassOneReferences.SingleOrDefault(x => x == "uomschedule").Should().NotBeNullOrEmpty();
            actual.PassOneReferences.SingleOrDefault(x => x == "queue").Should().NotBeNullOrEmpty();
        }
    }
}