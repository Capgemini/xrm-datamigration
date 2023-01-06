using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.CrmStore.DataStores;
using Capgemini.Xrm.DataMigration.FileStore.DataStore;
using Capgemini.Xrm.DataMigration.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmFileDataExporterCsvTests : UnitTestBase
    {
        private CrmSchemaConfiguration schemaConfig;
        private DataCrmStoreReader dataCrmStoreReader;
        private DataFileStoreWriterCsv dataCrmStoreWriter;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();

            var entities = new List<CrmEntity>() { new CrmEntity(), new CrmEntity() };

            schemaConfig = new CrmSchemaConfiguration();
            schemaConfig.Entities.AddRange(entities);

            MockEntityRepo.SetupGet(x => x.GetEntityMetadataCache).Returns(MockEntityMetadataCache.Object);
            MockCrmStoreReaderConfig.SetupGet(a => a.PageSize).Returns(500);
            MockCrmStoreReaderConfig.SetupGet(a => a.BatchSize).Returns(500);
            MockCrmStoreReaderConfig.SetupGet(a => a.TopCount).Returns(1000);
            MockCrmStoreReaderConfig.SetupGet(a => a.OneEntityPerBatch).Returns(true);
            MockCrmStoreReaderConfig.SetupGet(a => a.FieldsToObfuscate).Returns(EmptyFieldsToObfuscate);
            MockCrmStoreReaderConfig.Setup(a => a.GetFetchXMLQueries(It.IsAny<IEntityMetadataCache>())).Returns(new List<string>());

            dataCrmStoreReader = new DataCrmStoreReader(MockLogger.Object, MockEntityRepo.Object, MockCrmStoreReaderConfig.Object);

            string filePrefix = "Test";
            string filesPath = "TestData";
            List<string> excludedFields = new List<string>();

            dataCrmStoreWriter = new DataFileStoreWriterCsv(MockLogger.Object, filePrefix, filesPath, excludedFields, schemaConfig);
        }

        [TestMethod]
        public void CrmFileDataExporterCsv()
        {
            FluentActions.Invoking(() => new CrmFileDataExporterCsv(MockLogger.Object, dataCrmStoreReader, dataCrmStoreWriter))
               .Should()
               .NotThrow();
        }

        [TestMethod]
        public void CrmFileDataExporterCsvTest1()
        {
            FluentActions.Invoking(() => new CrmFileDataExporterCsv(MockLogger.Object, dataCrmStoreReader, dataCrmStoreWriter, CancellationToken.None))
               .Should()
               .NotThrow();
        }

        [TestMethod]
        public void CrmFileDataExporterCsvTest2()
        {
            MockEntityRepo.SetupGet(x => x.GetEntityMetadataCache).Returns(MockEntityMetadataCache.Object);

            MockCrmStoreReaderConfig.SetupGet(a => a.PageSize).Returns(50);
            MockCrmStoreReaderConfig.SetupGet(a => a.BatchSize).Returns(50);
            MockCrmStoreReaderConfig.SetupGet(a => a.TopCount).Returns(100);
            MockCrmStoreReaderConfig.SetupGet(a => a.OneEntityPerBatch).Returns(true);
            MockCrmStoreReaderConfig.Setup(a => a.GetFetchXMLQueries(It.IsAny<IEntityMetadataCache>())).Returns(new List<string>());

            MockFileStoreWriterConfig.SetupGet(a => a.FilePrefix).Returns("Test");
            MockFileStoreWriterConfig.SetupGet(a => a.JsonFolderPath).Returns("TestData");
            MockFileStoreWriterConfig.SetupGet(a => a.ExcludedFields).Returns(new List<string>());

            FluentActions.Invoking(() => new CrmFileDataExporterCsv(MockLogger.Object, MockEntityRepo.Object, MockCrmStoreReaderConfig.Object, MockFileStoreWriterConfig.Object, schemaConfig, CancellationToken.None))
               .Should()
               .NotThrow();

            MockCrmStoreReaderConfig.VerifyAll();
            MockFileStoreWriterConfig.VerifyAll();
        }

        [TestMethod]
        public void CrmFileDataExporterCsvTest3()
        {
            MockEntityRepo.SetupGet(x => x.GetEntityMetadataCache).Returns(MockEntityMetadataCache.Object);

            CrmExporterConfig crmExporterConfig = new CrmExporterConfig
            {
                PageSize = 50,
                BatchSize = 50,
                TopCount = 100,
                FilePrefix = "Test",
                JsonFolderPath = "TestData"
            };

            FluentActions.Invoking(() => new CrmFileDataExporterCsv(MockLogger.Object, MockEntityRepo.Object, crmExporterConfig, schemaConfig, CancellationToken.None))
               .Should()
               .NotThrow();
        }
    }
}