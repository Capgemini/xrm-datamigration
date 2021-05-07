using System;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Moq;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.DataProcessors
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class SkipExistingRecordProcessorTests : UnitTestBase
    {
        private readonly int passNumber = 1;
        private readonly int maxPassNumber = 3;
        private SkipExistingRecordProcessor systemUnderTest;
        private EntityWrapper originalEntity;
        private EntityWrapper newEntity;
        private EntityWrapper updatedEntity;
        private EntityWrapper unchangedEntity;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();

            originalEntity = new EntityWrapper(new Entity() { LogicalName = "testentity", Id = Guid.NewGuid() });
            newEntity = new EntityWrapper(new Entity() { LogicalName = "testentity", Id = Guid.NewGuid() });
            updatedEntity = new EntityWrapper(new Entity() { LogicalName = "testentity", Id = originalEntity.Id });
            unchangedEntity = new EntityWrapper(new Entity() { LogicalName = "testentity", Id = originalEntity.Id });

            originalEntity.OriginalEntity.Attributes["testattribute"] = "1";
            newEntity.OriginalEntity.Attributes["testattribute"] = "2";
            updatedEntity.OriginalEntity.Attributes["testattribute"] = "3";
            unchangedEntity.OriginalEntity.Attributes["testattribute"] = originalEntity.OriginalEntity.Attributes["testattribute"];

            var entityCollection = new EntityCollection();
            entityCollection.Entities.Add(originalEntity.OriginalEntity);

            MockEntityRepo.Setup(a => a.GetCurrentOrgService).Returns(MockOrganizationService.Object);

            MockOrganizationService.Setup(a => a.RetrieveMultiple(It.IsAny<QueryBase>()))
                           .Returns(entityCollection);

            systemUnderTest = new SkipExistingRecordProcessor(MockLogger.Object, MockEntityRepo.Object);
        }

        [TestMethod]
        public void ProcessOriginalEntity()
        {
            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(originalEntity, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();

            Assert.IsTrue(originalEntity.OperationType == Capgemini.DataMigration.Core.OperationType.Ignore);

            MockEntityRepo.VerifyAll();
        }

        [TestMethod]
        public void ProcessUpdatedEntity()
        {
            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(updatedEntity, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();

            Assert.IsFalse(updatedEntity.OperationType == Capgemini.DataMigration.Core.OperationType.Ignore);

            MockEntityRepo.VerifyAll();
        }

        [TestMethod]
        public void ProcessNewEntity()
        {
            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(newEntity, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();

            Assert.IsFalse(newEntity.OperationType == Capgemini.DataMigration.Core.OperationType.Ignore);

            MockEntityRepo.VerifyAll();
        }

        [TestMethod]
        public void ProcessUnchangedEntity()
        {
            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(unchangedEntity, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();
            Assert.IsTrue(unchangedEntity.OperationType == Capgemini.DataMigration.Core.OperationType.Ignore);

            MockEntityRepo.VerifyAll();
        }
    }
}