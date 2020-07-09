using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.CrmStore.DataStores;
using Capgemini.Xrm.DataMigration.FileStore.DataStore;
using Capgemini.Xrm.DataMigration.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmFileDataExporterTests : UnitTestBase
    {
        private CrmSchemaConfiguration schemaConfig;
        private DataCrmStoreReader dataCrmStoreReader;
        private DataFileStoreWriter dataFileStoreWriter;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();

            var entities = new List<CrmEntity>() { new CrmEntity(), new CrmEntity() };

            schemaConfig = new CrmSchemaConfiguration();
            schemaConfig.Entities.AddRange(entities);

            dataCrmStoreReader = new DataCrmStoreReader(MockLogger.Object, MockEntityRepo.Object, 500, 500, 1000, true, new List<string>(), EmptyFieldsToObfuscate);

            string filePrefix = "Test";
            string filesPath = "TestData";
            bool seperateFilesPerEntity = true;
            List<string> excludedFields = new List<string>();

            dataFileStoreWriter = new DataFileStoreWriter(MockLogger.Object, filePrefix, filesPath, excludedFields, seperateFilesPerEntity);
        }

        [TestMethod]
        public void CrmFileDataExporter()
        {
            FluentActions.Invoking(() => new CrmFileDataExporter(MockLogger.Object, dataCrmStoreReader, dataFileStoreWriter))
               .Should()
               .NotThrow();
        }

        [TestMethod]
        public void CrmFileDataExporterTest1()
        {
            FluentActions.Invoking(() => new CrmFileDataExporter(MockLogger.Object, dataCrmStoreReader, dataFileStoreWriter, CancellationToken.None))
               .Should()
               .NotThrow();
        }

        [TestMethod]
        public void CrmFileDataExporterTest2()
        {
            MockCrmStoreReaderConfig.SetupGet(a => a.PageSize).Returns(500);
            MockCrmStoreReaderConfig.SetupGet(a => a.BatchSize).Returns(500);
            MockCrmStoreReaderConfig.SetupGet(a => a.TopCount).Returns(1000);
            MockCrmStoreReaderConfig.SetupGet(a => a.OneEntityPerBatch).Returns(true);
            MockCrmStoreReaderConfig.Setup(a => a.GetFetchXMLQueries()).Returns(new List<string>());

            MockFileStoreWriterConfig.SetupGet(a => a.FilePrefix).Returns("Test");
            MockFileStoreWriterConfig.SetupGet(a => a.JsonFolderPath).Returns("TestData");
            MockFileStoreWriterConfig.SetupGet(a => a.ExcludedFields).Returns(new List<string>());

            FluentActions.Invoking(() => new CrmFileDataExporter(MockLogger.Object, MockEntityRepo.Object, MockCrmStoreReaderConfig.Object, MockFileStoreWriterConfig.Object, CancellationToken.None))
               .Should()
               .NotThrow();

            MockCrmStoreReaderConfig.VerifyAll();
            MockFileStoreWriterConfig.VerifyAll();
        }

        [TestMethod]
        public void CrmFileDataExporterTest3()
        {
            CrmExporterConfig crmExporterConfig = new CrmExporterConfig
            {
                PageSize = 50,
                BatchSize = 50,
                TopCount = 100,
                FilePrefix = "Test",
                JsonFolderPath = "TestData"
            };

            FluentActions.Invoking(() => new CrmFileDataExporter(MockLogger.Object, MockEntityRepo.Object, crmExporterConfig, CancellationToken.None))
               .Should()
               .NotThrow();
        }
    }
}