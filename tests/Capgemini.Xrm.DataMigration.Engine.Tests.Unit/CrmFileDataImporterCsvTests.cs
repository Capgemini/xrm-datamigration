using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.CrmStore.DataStores;
using Capgemini.Xrm.DataMigration.FileStore.DataStore;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmFileDataImporterCsvTests : UnitTestBase
    {
        private Mock<IFileStoreReaderConfig> mockReaderConfig;
        private Mock<ICrmGenericImporterConfig> mockCrmGenericImporterConfig;
        private CrmSchemaConfiguration schemaConfig;
        private CrmImportConfig crmImportConfig;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
            mockReaderConfig = new Mock<IFileStoreReaderConfig>();
            MockCrmStoreWriterConfig = new Mock<ICrmStoreWriterConfig>();
            mockCrmGenericImporterConfig = new Mock<ICrmGenericImporterConfig>();

            schemaConfig = new CrmSchemaConfiguration();
            crmImportConfig = new CrmImportConfig();
        }

        [TestMethod]
        public void CrmFileDataImporterConstructor1()
        {
            var dataFileStoreReader = new DataFileStoreReaderCsv(MockLogger.Object, "filePrefix", "TestData", new CrmSchemaConfiguration());
            var dataCrmStoreWriter = new DataCrmStoreWriter(MockLogger.Object, MockEntityRepo.Object);

            FluentActions.Invoking(() => new CrmFileDataImporterCsv(MockLogger.Object, dataFileStoreReader, dataCrmStoreWriter, mockCrmGenericImporterConfig.Object))
                       .Should()
                       .NotThrow();
        }

        [TestMethod]
        public void CrmFileDataImporterConstructor2()
        {
            var dataFileStoreReader = new DataFileStoreReaderCsv(MockLogger.Object, "filePrefix", "TestData", new CrmSchemaConfiguration());
            var dataCrmStoreWriter = new DataCrmStoreWriter(MockLogger.Object, MockEntityRepo.Object);

            FluentActions.Invoking(() => new CrmFileDataImporterCsv(MockLogger.Object, dataFileStoreReader, dataCrmStoreWriter, mockCrmGenericImporterConfig.Object, CancellationToken.None))
                       .Should()
                       .NotThrow();
        }

        [TestMethod]
        public void CrmFileDataImporterConstructor3()
        {
            mockReaderConfig.SetupProperty(a => a.FilePrefix, "Import");
            mockReaderConfig.SetupProperty(a => a.JsonFolderPath, "TestData");
            MockCrmStoreWriterConfig.SetupProperty(a => a.SaveBatchSize, 500);

            FluentActions.Invoking(() => new CrmFileDataImporterCsv(MockLogger.Object, MockEntityRepo.Object, mockReaderConfig.Object, MockCrmStoreWriterConfig.Object, mockCrmGenericImporterConfig.Object, schemaConfig, CancellationToken.None))
                       .Should()
                       .NotThrow();

            mockReaderConfig.VerifyGet(a => a.FilePrefix);
            mockReaderConfig.VerifyGet(a => a.JsonFolderPath);
            MockEntityRepo.VerifyAll();
            MockCrmStoreWriterConfig.VerifyGet(a => a.SaveBatchSize);
        }

        [TestMethod]
        public void CrmFileDataImporterConstructor4()
        {
            crmImportConfig.FilePrefix = "Import";
            crmImportConfig.JsonFolderPath = "TestData";

            FluentActions.Invoking(() => new CrmFileDataImporterCsv(MockLogger.Object, MockEntityRepo.Object, crmImportConfig, schemaConfig, CancellationToken.None))
                       .Should()
                       .NotThrow();
        }

        [TestMethod]
        public void CrmFileDataImporterConstructor5()
        {
            mockReaderConfig.SetupProperty(a => a.FilePrefix, "Import");
            mockReaderConfig.SetupProperty(a => a.JsonFolderPath, "TestData");
            MockCrmStoreWriterConfig.SetupProperty(a => a.SaveBatchSize, 500);

            FluentActions.Invoking(() => new CrmFileDataImporterCsv(MockLogger.Object, new List<IEntityRepository>() { MockEntityRepo.Object }, mockReaderConfig.Object, MockCrmStoreWriterConfig.Object, mockCrmGenericImporterConfig.Object, schemaConfig, CancellationToken.None))
                       .Should()
                       .NotThrow();

            mockReaderConfig.VerifyGet(a => a.FilePrefix);
            mockReaderConfig.VerifyGet(a => a.JsonFolderPath);
            MockEntityRepo.VerifyAll();
            MockCrmStoreWriterConfig.VerifyGet(a => a.SaveBatchSize);
        }

        [TestMethod]
        public void CrmFileDataImporterConstructor6()
        {
            crmImportConfig.FilePrefix = "Import";
            crmImportConfig.JsonFolderPath = "TestData";

            FluentActions.Invoking(() => new CrmFileDataImporterCsv(MockLogger.Object, new List<IEntityRepository>() { MockEntityRepo.Object }, crmImportConfig, schemaConfig, CancellationToken.None))
                       .Should()
                       .NotThrow();
        }
    }
}