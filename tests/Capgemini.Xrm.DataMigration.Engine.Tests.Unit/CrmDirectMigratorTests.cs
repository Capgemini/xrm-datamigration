using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.CrmStore.DataStores;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmDirectMigratorTests : UnitTestBase
    {
        private DataCrmStoreReader dataCrmStoreReader;
        private DataCrmStoreWriter dataCrmStoreWriter;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();

            dataCrmStoreReader = new DataCrmStoreReader(MockLogger.Object, MockEntityRepo.Object, 500, 500, 1000, true, new List<string>(), EmptyFieldsToObfuscate);

            dataCrmStoreWriter = new DataCrmStoreWriter(MockLogger.Object, MockEntityRepo.Object, 500, new List<string>(), new List<string>());
        }

        [TestMethod]
        public void CrmDirectMigrator()
        {
            FluentActions.Invoking(() => new CrmDirectMigrator(MockLogger.Object, dataCrmStoreReader, dataCrmStoreWriter, MockCrmGenericImporterConfig.Object))
                .Should()
                .NotThrow();
        }

        [TestMethod]
        public void CrmDirectMigratorTest1()
        {
            FluentActions.Invoking(() => new CrmDirectMigrator(MockLogger.Object, dataCrmStoreReader, dataCrmStoreWriter, MockCrmGenericImporterConfig.Object, CancellationToken.None))
                .Should()
                .NotThrow();
        }

        [TestMethod]
        public void CrmDirectMigratorTest2()
        {
            MockEntityRepo.SetupGet(x => x.GetEntityMetadataCache).Returns(MockEntityMetadataCache.Object);
            MockCrmStoreReaderConfig.SetupGet(a => a.PageSize).Returns(500);
            MockCrmStoreReaderConfig.SetupGet(a => a.BatchSize).Returns(500);
            MockCrmStoreReaderConfig.SetupGet(a => a.TopCount).Returns(1000);
            MockCrmStoreReaderConfig.SetupGet(a => a.OneEntityPerBatch).Returns(true);
            MockCrmStoreReaderConfig.Setup(a => a.GetFetchXMLQueries(It.IsAny<IEntityMetadataCache>())).Returns(new List<string>());

            MockCrmStoreWriterConfig.SetupGet(a => a.SaveBatchSize).Returns(10);

            FluentActions.Invoking(() => new CrmDirectMigrator(MockLogger.Object, MockEntityRepo.Object, MockCrmStoreReaderConfig.Object, MockCrmStoreWriterConfig.Object, MockCrmGenericImporterConfig.Object, CancellationToken.None))
                .Should()
                .NotThrow();
        }
    }
}