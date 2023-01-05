using System;
using System.Collections.Generic;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Moq;

namespace Capgemini.Xrm.DataMigration.CrmStore.DataStores.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class DataCrmStoreReaderTests : UnitTestBase
    {
        private readonly int pageSize = 100;
        private readonly int batchSize = 500;
        private readonly int topCount = 500;
        private readonly bool oneEntityPerBatch = true;

        private DataCrmStoreReader systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
        }

        [TestMethod]
        public void DataCrmStoreReaderConstructorNullLoggerInput()
        {
            FluentActions.Invoking(() => new DataCrmStoreReader(null, null, MockCrmStoreReaderConfig.Object))
                        .Should()
                        .Throw<ArgumentNullException>().WithMessage("*logger*");
        }

        [TestMethod]
        public void DataCrmStoreReaderConstructorNullEntityRepoInput()
        {
            FluentActions.Invoking(() => new DataCrmStoreReader(MockLogger.Object, null, MockCrmStoreReaderConfig.Object))
                        .Should()
                        .Throw<ArgumentNullException>().WithMessage("*entityRepo*");
        }

        [TestMethod]
        public void DataCrmStoreReaderConstructorPageSizeLessThan1()
        {
            MockCrmStoreReaderConfig.SetupGet(a => a.PageSize).Returns(0);

            FluentActions.Invoking(() => new DataCrmStoreReader(MockLogger.Object, MockEntityRepo.Object, MockCrmStoreReaderConfig.Object))
                        .Should()
                        .Throw<ArgumentOutOfRangeException>()
                        .Where(a => a.Message.Contains("Must be more than zero") && a.Message.Contains("PageSize"));

            MockCrmStoreReaderConfig.VerifyGet(a => a.PageSize);
        }

        [TestMethod]
        public void DataCrmStoreReaderConstructorBatchSizeLessThan1()
        {
            MockCrmStoreReaderConfig.SetupGet(a => a.PageSize).Returns(1);
            MockCrmStoreReaderConfig.SetupGet(a => a.BatchSize).Returns(0);

            FluentActions.Invoking(() => new DataCrmStoreReader(MockLogger.Object, MockEntityRepo.Object, MockCrmStoreReaderConfig.Object))
                        .Should()
                        .Throw<ArgumentOutOfRangeException>()
                        .Where(a => a.Message.Contains("Must be more than zero") && a.Message.Contains("BatchSize"));

            MockCrmStoreReaderConfig.VerifyGet(a => a.PageSize);
            MockCrmStoreReaderConfig.VerifyGet(a => a.BatchSize);
        }

        [TestMethod]
        public void DataCrmStoreReaderConstructorTopCountLessThan1()
        {
            MockCrmStoreReaderConfig.SetupGet(a => a.PageSize).Returns(1);
            MockCrmStoreReaderConfig.SetupGet(a => a.BatchSize).Returns(1);
            MockCrmStoreReaderConfig.SetupGet(a => a.TopCount).Returns(0);

            FluentActions.Invoking(() => new DataCrmStoreReader(MockLogger.Object, MockEntityRepo.Object, MockCrmStoreReaderConfig.Object))
                        .Should()
                        .Throw<ArgumentOutOfRangeException>()
                        .Where(a => a.Message.Contains("Must be more than zero") && a.Message.Contains("TopCount"));

            MockCrmStoreReaderConfig.VerifyGet(a => a.PageSize);
            MockCrmStoreReaderConfig.VerifyGet(a => a.BatchSize);
            MockCrmStoreReaderConfig.VerifyGet(a => a.TopCount);
        }

        [TestMethod]
        public void DataCrmStoreReaderConstructorPageSizeGreaterThanBatchSize()
        {
            MockCrmStoreReaderConfig.SetupGet(a => a.PageSize).Returns(2);
            MockCrmStoreReaderConfig.SetupGet(a => a.BatchSize).Returns(1);
            MockCrmStoreReaderConfig.SetupGet(a => a.TopCount).Returns(1);

            FluentActions.Invoking(() => new DataCrmStoreReader(MockLogger.Object, MockEntityRepo.Object, MockCrmStoreReaderConfig.Object))
                        .Should()
                        .Throw<ArgumentOutOfRangeException>()
                        .Where(a => a.Message.Contains("Must be less than or equal to batchSize") && a.Message.Contains("PageSize"));

            MockCrmStoreReaderConfig.VerifyGet(a => a.PageSize);
            MockCrmStoreReaderConfig.VerifyGet(a => a.BatchSize);
            MockCrmStoreReaderConfig.VerifyGet(a => a.TopCount);
        }

        [TestMethod]
        public void DataCrmStoreReaderConstructorTopCountLessThanBatchSize()
        {
            MockCrmStoreReaderConfig.SetupGet(a => a.PageSize).Returns(2);
            MockCrmStoreReaderConfig.SetupGet(a => a.BatchSize).Returns(6);
            MockCrmStoreReaderConfig.SetupGet(a => a.TopCount).Returns(4);

            FluentActions.Invoking(() => new DataCrmStoreReader(MockLogger.Object, MockEntityRepo.Object, MockCrmStoreReaderConfig.Object))
                        .Should()
                        .Throw<ArgumentOutOfRangeException>()
                        .Where(a => a.Message.Contains("Must be more than or equal to batchSize") && a.Message.Contains("TopCount"));

            MockCrmStoreReaderConfig.VerifyGet(a => a.PageSize);
            MockCrmStoreReaderConfig.VerifyGet(a => a.BatchSize);
            MockCrmStoreReaderConfig.VerifyGet(a => a.TopCount);
        }

        [TestMethod]
        public void DataCrmStoreReaderConstructorGetFetchXMLQueriesIsNull()
        {
            MockCrmStoreReaderConfig.SetupGet(a => a.PageSize).Returns(2);
            MockCrmStoreReaderConfig.SetupGet(a => a.BatchSize).Returns(4);
            MockCrmStoreReaderConfig.SetupGet(a => a.TopCount).Returns(4);

            FluentActions.Invoking(() => new DataCrmStoreReader(MockLogger.Object, MockEntityRepo.Object, MockCrmStoreReaderConfig.Object))
                        .Should()
                        .Throw<ArgumentNullException>().WithMessage("*fetchXmlQueries*");

            MockCrmStoreReaderConfig.VerifyGet(a => a.PageSize);
            MockCrmStoreReaderConfig.VerifyGet(a => a.BatchSize);
            MockCrmStoreReaderConfig.VerifyGet(a => a.TopCount);
        }

        [TestMethod]
        public void DataCrmStoreReaderConstructorWithConfigParameters()
        {
            MockEntityRepo.SetupGet(x => x.GetEntityMetadataCache).Returns(MockEntityMetadataCache.Object);
            MockCrmStoreReaderConfig.SetupGet(a => a.PageSize).Returns(2);
            MockCrmStoreReaderConfig.SetupGet(a => a.BatchSize).Returns(4);
            MockCrmStoreReaderConfig.SetupGet(a => a.TopCount).Returns(4);

            MockCrmStoreReaderConfig.Setup(a => a.GetFetchXMLQueries(It.IsAny<IEntityMetadataCache>())).Returns(FetchXMlQueries);

            FluentActions.Invoking(() => new DataCrmStoreReader(MockLogger.Object, MockEntityRepo.Object, MockCrmStoreReaderConfig.Object))
                        .Should()
                        .NotThrow();

            MockCrmStoreReaderConfig.VerifyGet(a => a.PageSize);
            MockCrmStoreReaderConfig.VerifyGet(a => a.BatchSize);
            MockCrmStoreReaderConfig.VerifyGet(a => a.TopCount);
            MockCrmStoreReaderConfig.VerifyGet(a => a.OneEntityPerBatch);
        }

        [TestMethod]
        public void DataCrmStoreReaderConstructor()
        {
            FluentActions.Invoking(() => new DataCrmStoreReader(MockLogger.Object, MockEntityRepo.Object, pageSize, batchSize, topCount, oneEntityPerBatch, FetchXMlQueries, EmptyFieldsToObfuscate))
                   .Should()
                   .NotThrow();
        }

        [TestMethod]
        public void ReadBatchDataFromStore()
        {
            var entityWrapperList = new List<EntityWrapper>
            {
                new EntityWrapper(new Entity("contact", Guid.NewGuid()))
            };

            MockEntityRepo.Setup(a => a.GetEntitesByFetchXML(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), ref It.Ref<string>.IsAny))
                           .Returns(entityWrapperList);

            systemUnderTest = new DataCrmStoreReader(MockLogger.Object, MockEntityRepo.Object, pageSize, batchSize, topCount, oneEntityPerBatch, FetchXMlQueries, EmptyFieldsToObfuscate);

            var actual = systemUnderTest.ReadBatchDataFromStore();

            actual.Should().NotBeNull();
            MockLogger.Verify(a => a.LogVerbose(It.IsAny<string>()));
        }

        [TestMethod]
        public void Reset()
        {
            systemUnderTest = new DataCrmStoreReader(MockLogger.Object, MockEntityRepo.Object, pageSize, batchSize, topCount, oneEntityPerBatch, FetchXMlQueries, EmptyFieldsToObfuscate);

            FluentActions.Invoking(() => systemUnderTest.Reset())
                        .Should()
                        .NotThrow();

            MockLogger.VerifyAll();
        }
    }
}