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
    public class CrmFileDataImporterTests : UnitTestBase
    {
        private Mock<IFileStoreReaderConfig> mockReaderConfig;
        private Mock<ICrmGenericImporterConfig> mockCrmGenericImporterConfig;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
            mockReaderConfig = new Mock<IFileStoreReaderConfig>();
            MockCrmStoreWriterConfig = new Mock<ICrmStoreWriterConfig>();
            mockCrmGenericImporterConfig = new Mock<ICrmGenericImporterConfig>();
        }

        [TestMethod]
        public void CrmFileDataImporterConstructor1()
        {
            var dataFileStoreReader = new DataFileStoreReader(MockLogger.Object, "Import", "TestData");
            var dataCrmStoreWriter = new DataCrmStoreWriter(MockLogger.Object, MockEntityRepo.Object);

            FluentActions.Invoking(() => new CrmFileDataImporter(MockLogger.Object, dataFileStoreReader, dataCrmStoreWriter, mockCrmGenericImporterConfig.Object))
                       .Should()
                       .NotThrow();
        }

        [TestMethod]
        public void CrmFileDataImporterConstructor2()
        {
            var dataFileStoreReader = new DataFileStoreReader(MockLogger.Object, "Import", "TestData");
            var dataCrmStoreWriter = new DataCrmStoreWriter(MockLogger.Object, MockEntityRepo.Object);

            FluentActions.Invoking(() => new CrmFileDataImporter(MockLogger.Object, dataFileStoreReader, dataCrmStoreWriter, mockCrmGenericImporterConfig.Object, CancellationToken.None))
                       .Should()
                       .NotThrow();

            mockReaderConfig.VerifyAll();
            MockEntityRepo.VerifyAll();
        }

        [TestMethod]
        public void CrmFileDataImporterConstructor3()
        {
            mockReaderConfig.SetupProperty(a => a.FilePrefix, "Import");
            mockReaderConfig.SetupProperty(a => a.JsonFolderPath, "TestData");
            MockCrmStoreWriterConfig.SetupProperty(a => a.SaveBatchSize, 500);

            FluentActions.Invoking(() => new CrmFileDataImporter(MockLogger.Object, MockEntityRepo.Object, mockReaderConfig.Object, MockCrmStoreWriterConfig.Object, mockCrmGenericImporterConfig.Object, CancellationToken.None))
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
            var importConfig = new CrmImportConfig
            {
                FilePrefix = "Import",
                JsonFolderPath = "TestData",
                SaveBatchSize = 500
            };

            FluentActions.Invoking(() => new CrmFileDataImporter(MockLogger.Object, MockEntityRepo.Object, importConfig, CancellationToken.None))
                       .Should()
                       .NotThrow();
        }

        [TestMethod]
        public void CrmFileDataImporterConstructor5()
        {
            mockReaderConfig.SetupProperty(a => a.FilePrefix, "Import");
            mockReaderConfig.SetupProperty(a => a.JsonFolderPath, "TestData");
            MockCrmStoreWriterConfig.SetupProperty(a => a.SaveBatchSize, 500);

            FluentActions.Invoking(() => new CrmFileDataImporter(MockLogger.Object, new List<IEntityRepository> { MockEntityRepo.Object }, mockReaderConfig.Object, MockCrmStoreWriterConfig.Object, mockCrmGenericImporterConfig.Object, CancellationToken.None))
                       .Should()
                       .NotThrow();

            mockReaderConfig.VerifyGet(a => a.FilePrefix);
            mockReaderConfig.VerifyGet(a => a.JsonFolderPath);
            MockCrmStoreWriterConfig.VerifyGet(a => a.SaveBatchSize);
        }

        [TestMethod]
        public void CrmFileDataImporterConstructor6()
        {
            var importConfig = new CrmImportConfig
            {
                FilePrefix = "Import",
                JsonFolderPath = "TestData",
                SaveBatchSize = 500
            };

            FluentActions.Invoking(() => new CrmFileDataImporter(MockLogger.Object, new List<IEntityRepository>() { MockEntityRepo.Object }, importConfig, CancellationToken.None))
                       .Should()
                       .NotThrow();
        }
    }
}