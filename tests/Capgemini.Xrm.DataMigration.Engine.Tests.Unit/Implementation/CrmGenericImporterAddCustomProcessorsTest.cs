using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.CrmStore.DataStores;
using Capgemini.Xrm.DataMigration.DataStore;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Moq;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.Implementation
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmGenericImporterAddCustomProcessorsTest : UnitTestBase
    {
        private Mock<IDataStoreReader<Entity, EntityWrapper>> mockStoreReader;
        private DataCrmStoreWriter storeWriter;
        private ObjectTypeCodeMappingConfiguration objectTypeCodeMappingConfig;

        [TestInitialize]
        public void Initialize()
        {
            InitializeProperties();

            mockStoreReader = new Mock<IDataStoreReader<Entity, EntityWrapper>>();
            storeWriter = new DataCrmStoreWriter(MockLogger.Object, MockEntityRepo.Object);
            objectTypeCodeMappingConfig = new ObjectTypeCodeMappingConfiguration();
        }

        [TestMethod]
        public void AddCustomProcessorsObjectTypeCodeConfigAddsObjectTypeCodeProcessor()
        {
            string expectedLoggerOutput = "Using ObjectTypeCodeProcessor processor";

            MockCrmGenericImporterConfig.SetupGet(c => c.ObjectTypeCodeMappingConfig).Returns(objectTypeCodeMappingConfig);

            var importer = new TestCrmGenericImporter(MockLogger.Object, mockStoreReader.Object, storeWriter, MockCrmGenericImporterConfig.Object);

            Assert.IsNotNull(importer);

            MockLogger.Verify(l => l.LogVerbose(expectedLoggerOutput), Times.Once);
            MockCrmGenericImporterConfig.VerifyAll();
        }

        [TestMethod]
        public void FiledsToIgnore()
        {
            List<string> fieldsToIgnore = new List<string>() { "contactid" };

            MockCrmGenericImporterConfig.SetupGet(c => c.ObjectTypeCodeMappingConfig).Returns(new ObjectTypeCodeMappingConfiguration());

            MockCrmGenericImporterConfig.SetupGet(c => c.FiledsToIgnore).Returns(fieldsToIgnore);

            var importer = new TestCrmGenericImporter(MockLogger.Object, mockStoreReader.Object, storeWriter, MockCrmGenericImporterConfig.Object);

            importer.Should().NotBeNull();
            MockCrmGenericImporterConfig.VerifyAll();
        }

        [TestMethod]
        public void NoUpdateEntities()
        {
            List<string> noUpdateEntities = new List<string>() { "contactid" };

            MockCrmGenericImporterConfig.SetupGet(c => c.ObjectTypeCodeMappingConfig).Returns(new ObjectTypeCodeMappingConfiguration());

            MockCrmGenericImporterConfig.SetupGet(c => c.NoUpdateEntities).Returns(noUpdateEntities);

            var importer = new TestCrmGenericImporter(MockLogger.Object, mockStoreReader.Object, storeWriter, MockCrmGenericImporterConfig.Object);

            importer.Should().NotBeNull();
            MockCrmGenericImporterConfig.VerifyAll();
        }

        [TestMethod]
        public void EntitiesToSync()
        {
            List<string> entitiesToSync = new List<string>() { "contactid" };

            MockCrmGenericImporterConfig.SetupGet(c => c.ObjectTypeCodeMappingConfig).Returns(new ObjectTypeCodeMappingConfiguration());

            MockCrmGenericImporterConfig.SetupGet(c => c.EntitiesToSync).Returns(entitiesToSync);

            var importer = new TestCrmGenericImporter(MockLogger.Object, mockStoreReader.Object, storeWriter, MockCrmGenericImporterConfig.Object);

            importer.Should().NotBeNull();
            MockCrmGenericImporterConfig.VerifyAll();
        }

        [TestMethod]
        public void DeactivateAllProcesses()
        {
            MockCrmGenericImporterConfig.SetupGet(c => c.ObjectTypeCodeMappingConfig).Returns(new ObjectTypeCodeMappingConfiguration());

            MockCrmGenericImporterConfig.SetupGet(c => c.DeactivateAllProcesses).Returns(true);
            MockCrmGenericImporterConfig.Setup(a => a.ResetProcessesToDeactivate());
            MockCrmGenericImporterConfig.Setup(a => a.ResetPluginsToDeactivate());

            var importer = new TestCrmGenericImporter(MockLogger.Object, mockStoreReader.Object, storeWriter, MockCrmGenericImporterConfig.Object);

            importer.Should().NotBeNull();
            MockCrmGenericImporterConfig.VerifyAll();
        }

        [TestMethod]
        public void ProcessesToDeactivate()
        {
            List<string> processesToDeactivate = new List<string>() { "contactid" };

            MockCrmGenericImporterConfig.SetupGet(c => c.ObjectTypeCodeMappingConfig).Returns(new ObjectTypeCodeMappingConfiguration());

            MockCrmGenericImporterConfig.SetupGet(c => c.ProcessesToDeactivate).Returns(processesToDeactivate);
            MockCrmGenericImporterConfig.Setup(a => a.ResetPluginsToDeactivate());

            var importer = new TestCrmGenericImporter(MockLogger.Object, mockStoreReader.Object, storeWriter, MockCrmGenericImporterConfig.Object);

            importer.Should().NotBeNull();

            MockCrmGenericImporterConfig.VerifyAll();
        }
    }
}