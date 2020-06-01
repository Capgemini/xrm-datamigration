using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.DataStore;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.CrmStore.DataStores.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DataCrmStoreWriterTests : UnitTestBase
    {
        private DataCrmStoreWriter systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
        }

        [TestMethod]
        public void DataCrmStoreWriterConstructor()
        {
            MockCrmStoreWriterConfig.SetupProperty(a => a.SaveBatchSize, 12);
            MockCrmStoreWriterConfig.SetupGet(a => a.NoUpsertEntities).Returns(new List<string>());
            MockCrmStoreWriterConfig.SetupGet(a => a.NoUpdateEntities).Returns(new List<string>());

            FluentActions.Invoking(() => new DataCrmStoreWriter(MockLogger.Object, MockEntityRepo.Object, MockCrmStoreWriterConfig.Object, CancellationToken.None))
                    .Should()
                    .NotThrow();
        }

        [TestMethod]
        public void DataCrmStoreWriterConstructorWithCancellationToken()
        {
            MockCrmStoreWriterConfig.SetupProperty(a => a.SaveBatchSize, 12);
            MockCrmStoreWriterConfig.SetupGet(a => a.NoUpsertEntities).Returns(new List<string>());
            MockCrmStoreWriterConfig.SetupGet(a => a.NoUpdateEntities).Returns(new List<string>());

            FluentActions.Invoking(() => new DataCrmStoreWriter(MockLogger.Object, MockEntityRepo.Object, MockCrmStoreWriterConfig.Object))
                   .Should()
                   .NotThrow();
        }

        [TestMethod]
        public void DataCrmStoreWriterConstructorNullLogger()
        {
            int savePageSize = 500;
            List<string> noUpsertEntities = null;
            List<string> noUpdateEntities = null;

            FluentActions.Invoking(() => new DataCrmStoreWriter(null, MockEntityRepo.Object, savePageSize, noUpsertEntities, noUpdateEntities))
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithMessage("*logger*");
        }

        [TestMethod]
        public void DataCrmStoreWriterConstructorNullEntityRepo()
        {
            int savePageSize = 500;
            List<string> noUpsertEntities = null;
            List<string> noUpdateEntities = null;

            FluentActions.Invoking(() => new DataCrmStoreWriter(MockLogger.Object, null, savePageSize, noUpsertEntities, noUpdateEntities))
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithMessage("*entityRepo*");
        }

        [TestMethod]
        public void DataCrmStoreWriterConstructorSavePageSizeEqualZero()
        {
            int savePageSize = 0;
            List<string> noUpsertEntities = null;
            List<string> noUpdateEntities = null;

            FluentActions.Invoking(() => new DataCrmStoreWriter(MockLogger.Object, MockEntityRepo.Object, savePageSize, noUpsertEntities, noUpdateEntities))
                   .Should()
                   .Throw<ArgumentOutOfRangeException>()
                   .WithMessage("*savePageSize*");
        }

        [TestMethod]
        public void DataCrmStoreWriterConstructorValidParameters()
        {
            int savePageSize = 500;
            List<string> noUpsertEntities = null;
            List<string> noUpdateEntities = null;

            FluentActions.Invoking(() => new DataCrmStoreWriter(MockLogger.Object, MockEntityRepo.Object, savePageSize, noUpsertEntities, noUpdateEntities))
                   .Should()
                   .NotThrow();
        }

        [TestMethod]
        public void SaveBatchDataToStoreCreate()
        {
            var accountName = "account";
            int savePageSize = 3;
            List<string> noUpsertEntities = null;
            List<string> noUpdateEntities = null;

            var testAccount = new EntityWrapper(new Entity(accountName, Guid.NewGuid()));
            testAccount.OriginalEntity.Attributes["ownerid"] = Guid.NewGuid();
            testAccount.OperationType = OperationType.Create;

            var entityList = new List<EntityWrapper>
            {
                new EntityWrapper(new Entity(accountName, Guid.NewGuid())),
                new EntityWrapper(new Entity(accountName, Guid.NewGuid())),
                testAccount
            };

            systemUnderTest = new DataCrmStoreWriter(MockLogger.Object, MockEntityRepo.Object, savePageSize, noUpsertEntities, noUpdateEntities);

            FluentActions.Invoking(() => systemUnderTest.SaveBatchDataToStore(entityList))
                           .Should()
                           .NotThrow();
        }

        [TestMethod]
        public void SaveBatchDataToStoreAssign()
        {
            var accountName = "account";
            int savePageSize = 3;
            List<string> noUpsertEntities = null;
            List<string> noUpdateEntities = null;

            var testAccount = new EntityWrapper(new Entity(accountName, Guid.NewGuid()));
            testAccount.OriginalEntity.Attributes["ownerid"] = Guid.NewGuid();
            testAccount.OperationType = OperationType.Assign;

            var entityList = new List<EntityWrapper>
            {
                new EntityWrapper(new Entity(accountName, Guid.NewGuid())),
                new EntityWrapper(new Entity(accountName, Guid.NewGuid())),
                testAccount
            };

            systemUnderTest = new DataCrmStoreWriter(MockLogger.Object, MockEntityRepo.Object, savePageSize, noUpsertEntities, noUpdateEntities);

            FluentActions.Invoking(() => systemUnderTest.SaveBatchDataToStore(entityList))
                           .Should()
                           .NotThrow();
        }

        [TestMethod]
        public void SaveBatchDataToStoreUpdate()
        {
            var accountName = "account";
            int savePageSize = 3;
            List<string> noUpsertEntities = null;
            List<string> noUpdateEntities = null;

            var testAccount = new EntityWrapper(new Entity(accountName, Guid.NewGuid()));
            testAccount.OriginalEntity.Attributes["ownerid"] = Guid.NewGuid();
            testAccount.OperationType = OperationType.Update;

            var entityList = new List<EntityWrapper>
            {
                new EntityWrapper(new Entity(accountName, Guid.NewGuid())),
                new EntityWrapper(new Entity(accountName, Guid.NewGuid())),
                testAccount
            };

            systemUnderTest = new DataCrmStoreWriter(MockLogger.Object, MockEntityRepo.Object, savePageSize, noUpsertEntities, noUpdateEntities);

            FluentActions.Invoking(() => systemUnderTest.SaveBatchDataToStore(entityList))
                           .Should()
                           .NotThrow();
        }

        [TestMethod]
        public void SaveBatchDataToStoreManyToMany()
        {
            var accountName = "account";
            int savePageSize = 1;

            var testManyToMany = new EntityWrapper(new Entity("accountcontact", Guid.NewGuid()), true)
            {
                OperationType = OperationType.Associate
            };

            var testAccount = new EntityWrapper(new Entity(accountName, Guid.NewGuid()), true)
            {
                OperationType = OperationType.Create
            };

            var entityList = new List<EntityWrapper>
            {
                testAccount,
                testManyToMany
            };

            var noUpsertEntities = new List<string>();
            var noUpdateEntities = new List<string>();

            systemUnderTest = new DataCrmStoreWriter(MockLogger.Object, MockEntityRepo.Object, savePageSize, noUpsertEntities, noUpdateEntities);

            FluentActions.Invoking(() => systemUnderTest.SaveBatchDataToStore(entityList))
                           .Should()
                           .NotThrow();
        }

        [TestMethod]
        public void SaveBatchDataToStoreOperationTypeFailed()
        {
            var accountName = "account";
            int savePageSize = 1;

            var testManyToMany = new EntityWrapper(new Entity("accountcontact", Guid.NewGuid()), true)
            {
                OperationType = OperationType.Failed,
                OperationResult = "Failed"
            };

            var testAccount = new EntityWrapper(new Entity(accountName, Guid.NewGuid()), true)
            {
                OperationType = OperationType.Failed,
                OperationResult = "Failed"
            };

            var entityList = new List<EntityWrapper>
            {
                testAccount,
                testManyToMany
            };

            var noUpsertEntities = new List<string>();
            var noUpdateEntities = new List<string>();

            systemUnderTest = new DataCrmStoreWriter(MockLogger.Object, MockEntityRepo.Object, savePageSize, noUpsertEntities, noUpdateEntities);

            FluentActions.Invoking(() => systemUnderTest.SaveBatchDataToStore(entityList))
                           .Should()
                           .NotThrow();
        }

        [TestMethod]
        public void Reset()
        {
            int savePageSize = 500;
            List<string> noUpsertEntities = null;
            List<string> noUpdateEntities = null;

            systemUnderTest = new DataCrmStoreWriter(MockLogger.Object, MockEntityRepo.Object, savePageSize, noUpsertEntities, noUpdateEntities);

            FluentActions.Invoking(() => systemUnderTest.Reset())
                           .Should()
                           .NotThrow();
        }
    }
}