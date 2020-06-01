using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.DataProcessors
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class IgnoredFieldsProcessorTests
    {
        private IgnoredFieldsProcessor systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            systemUnderTest = new IgnoredFieldsProcessor(new List<string>() { "dateofbirth" });
        }

        [TestMethod]
        public void IgnoredFieldsProcessor()
        {
            FluentActions.Invoking(() => new IgnoredFieldsProcessor(new List<string>()))
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ProcessEntityNullEntityWrapper()
        {
            EntityWrapper entity = null;
            int passNumber = 1;
            int maxPassNumber = 3;

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entity, passNumber, maxPassNumber))
                         .Should()
                         .Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ProcessEntityWithOperationTypeOfIgnore()
        {
            var entity = new EntityWrapper(new Entity())
            {
                OperationType = OperationType.Ignore
            };
            int passNumber = 1;
            int maxPassNumber = 3;

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entity, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ProcessEntity()
        {
            var originalEntity = new Entity("contact");
            originalEntity.Attributes["firstname"] = "Joe";
            originalEntity.Attributes["lastname"] = "Bloggs";
            originalEntity.Attributes["dateofbirth"] = DateTime.UtcNow;

            var entityWrapper = new EntityWrapper(originalEntity)
            {
                OperationType = OperationType.Create
            };
            int passNumber = 1;
            int maxPassNumber = 3;

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entityWrapper, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ProcessEntityWithAttributeToBeDeleted()
        {
            var originalEntity = new Entity("contact");
            originalEntity.Attributes["firstname"] = "Joe";
            originalEntity.Attributes["lastname"] = "Bloggs";
            originalEntity.Attributes["dateofbirth"] = DateTime.UtcNow;
            originalEntity.Attributes["BE DELETEDheight"] = DateTime.UtcNow;
            var entityWrapper = new EntityWrapper(originalEntity)
            {
                OperationType = OperationType.Create
            };
            int passNumber = 1;
            int maxPassNumber = 3;

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entityWrapper, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ProcessEntityWithAttributesHavingEntityCollection()
        {
            var entityCollection = new EntityCollection(
                new List<Entity>
                {
                 new Entity("account", Guid.NewGuid()),
                 new Entity("account", Guid.NewGuid())
                });

            var originalEntity = new Entity("contact");
            originalEntity.Attributes["firstname"] = "Joe";
            originalEntity.Attributes["lastname"] = "Bloggs";
            originalEntity.Attributes["dateofbirth"] = DateTime.UtcNow;
            originalEntity.Attributes["contactaccounts"] = entityCollection;

            var entityWrapper = new EntityWrapper(originalEntity)
            {
                OperationType = OperationType.Create
            };
            int passNumber = 1;
            int maxPassNumber = 3;

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entityWrapper, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ImportCompleted()
        {
            FluentActions.Invoking(() => systemUnderTest.ImportCompleted())
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ImportStarted()
        {
            FluentActions.Invoking(() => systemUnderTest.ImportStarted())
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void MinRequiredPassNumber()
        {
            var actual = systemUnderTest.MinRequiredPassNumber;

            actual.Should().Be(1);
        }
    }
}