using System;
using System.Collections.Generic;
using System.Threading;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.DataStore;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.CrmStore.DataStores.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class DataCrmStoreWriterMultiThreadedTests : UnitTestBase
    {
        private DataCrmStoreWriterMultiThreaded systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
        }

        [TestMethod]
        public void DataCrmStoreWriterMultiThreaded()
        {
            MockCrmStoreWriterConfig.SetupGet(a => a.SaveBatchSize).Returns(12);
            MockCrmStoreWriterConfig.SetupGet(a => a.NoUpsertEntities).Returns(new List<string>());
            MockCrmStoreWriterConfig.SetupGet(a => a.NoUpdateEntities).Returns(new List<string>());

            FluentActions.Invoking(() => new DataCrmStoreWriterMultiThreaded(MockLogger.Object, new List<Core.IEntityRepository> { MockEntityRepo.Object }, MockCrmStoreWriterConfig.Object, CancellationToken.None))
                    .Should()
                    .NotThrow();

            MockCrmStoreWriterConfig.VerifyAll();
        }

        [TestMethod]
        public void DataCrmStoreWriterMultiThreadedSecondConstructor()
        {
            MockCrmStoreWriterConfig.SetupGet(a => a.SaveBatchSize).Returns(12);
            MockCrmStoreWriterConfig.SetupGet(a => a.NoUpsertEntities).Returns(new List<string>());
            MockCrmStoreWriterConfig.SetupGet(a => a.NoUpdateEntities).Returns(new List<string>());

            FluentActions.Invoking(() => new DataCrmStoreWriterMultiThreaded(MockLogger.Object, new List<Core.IEntityRepository> { MockEntityRepo.Object }, MockCrmStoreWriterConfig.Object))
                    .Should()
                    .NotThrow();

            MockCrmStoreWriterConfig.VerifyAll();
        }

        [TestMethod]
        public void DataCrmStoreWriterMultiThreadedSecondConstructorNullCrmStoreWriterConfig()
        {
            FluentActions.Invoking(() => new DataCrmStoreWriterMultiThreaded(MockLogger.Object, new List<Core.IEntityRepository> { MockEntityRepo.Object }, null))
                    .Should()
                    .Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void DataCrmStoreWriterMultiThreadedSecondConstructorSaveBatchSizeEqualZero()
        {
            MockCrmStoreWriterConfig.SetupGet(a => a.SaveBatchSize).Returns(0);
            MockCrmStoreWriterConfig.SetupGet(a => a.NoUpsertEntities).Returns(new List<string>());
            MockCrmStoreWriterConfig.SetupGet(a => a.NoUpdateEntities).Returns(new List<string>());

            FluentActions.Invoking(() => new DataCrmStoreWriterMultiThreaded(MockLogger.Object, new List<Core.IEntityRepository> { MockEntityRepo.Object }, null))
                    .Should()
                    .Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void DataCrmStoreWriterMultiThreadedTest2()
        {
            int savePageSize = 500;
            List<string> noUpsertEntities = null;
            List<string> noUpdateEntities = null;

            FluentActions.Invoking(() => new DataCrmStoreWriterMultiThreaded(MockLogger.Object, new List<Core.IEntityRepository> { MockEntityRepo.Object }, savePageSize, noUpsertEntities, noUpdateEntities))
                          .Should()
                          .NotThrow();
        }

        [TestMethod]
        public void SaveBatchDataToStore()
        {
            int savePageSize = 3;
            List<string> noUpsertEntities = null;
            List<string> noUpdateEntities = null;

            var entityList = new List<EntityWrapper>
            {
                new EntityWrapper(new Entity("account", Guid.NewGuid())),
                new EntityWrapper(new Entity("account", Guid.NewGuid())),
                new EntityWrapper(new Entity("account", Guid.NewGuid())),
                new EntityWrapper(new Entity("account", Guid.NewGuid()))
            };

            systemUnderTest = new DataCrmStoreWriterMultiThreaded(MockLogger.Object, new List<Core.IEntityRepository> { MockEntityRepo.Object }, savePageSize, noUpsertEntities, noUpdateEntities);

            FluentActions.Invoking(() => systemUnderTest.SaveBatchDataToStore(entityList))
                          .Should()
                          .NotThrow();
        }
    }
}