using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
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
        private string entityName = "account";

        private SkipExistingRecordProcessor systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
        }

        [TestMethod]
        public void ProcessEntity()
        {
            EntityWrapper originalEntity = new EntityWrapper(new Entity() { LogicalName = "testentity", Id = Guid.NewGuid() });
            EntityWrapper newEntity = new EntityWrapper(new Entity() { LogicalName = "testentity", Id = Guid.NewGuid() });
            EntityWrapper updatedEntity = new EntityWrapper(new Entity() { LogicalName = "testentity", Id = originalEntity.Id });
            EntityWrapper unchangedEntity = new EntityWrapper(new Entity() { LogicalName = "testentity", Id = originalEntity.Id });
            var entityMetadata = new EntityMetadata();

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

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(originalEntity, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(newEntity, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(updatedEntity, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(unchangedEntity, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();

            Assert.IsTrue(originalEntity.OperationType == Capgemini.DataMigration.Core.OperationType.Ignore);
            Assert.IsFalse(newEntity.OperationType == Capgemini.DataMigration.Core.OperationType.Ignore);
            Assert.IsFalse(updatedEntity.OperationType == Capgemini.DataMigration.Core.OperationType.Ignore);
            Assert.IsTrue(unchangedEntity.OperationType == Capgemini.DataMigration.Core.OperationType.Ignore);

            MockEntityRepo.VerifyAll();
        }
    }
}