using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using Capgemini.Xrm.DataMigration.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.DataProcessors
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class PassZeroReferenceProcessorTests : UnitTestBase
    {
        private readonly List<string> passOneReferences = new List<string> { "passoneFirst", "passoneSecond" };

        private PassZeroReferenceProcessor systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();

            systemUnderTest = new PassZeroReferenceProcessor(passOneReferences, MockLogger.Object);
        }

        [TestMethod]
        public void PassZeroReferenceProcessor()
        {
            FluentActions.Invoking(() => new PassZeroReferenceProcessor(passOneReferences, MockLogger.Object))
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
        public void ProcessEntity()
        {
            EntityWrapper entity = new EntityWrapper(new Entity());
            int passNumber = 1;
            int maxPassNumber = 3;

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entity, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ProcessEntityPassZeroZeroPassEntity()
        {
            Entity entity = new Entity(passOneReferences.First());
            EntityWrapper entityWrapper = new EntityWrapper(entity);

            PassType pass = PassType.CreateRequiredEntity;

            systemUnderTest.ProcessEntity(entityWrapper, (int)pass, 100);

            Assert.AreEqual(OperationType.New, entityWrapper.OperationType);
        }

        [TestMethod]
        public void ProcessEntityPassZeroOtherBusinessEntity()
        {
            Entity entity = new Entity("some business entity");
            EntityWrapper entityWrapper = new EntityWrapper(entity);

            PassType pass = PassType.CreateRequiredEntity;

            systemUnderTest.ProcessEntity(entityWrapper, (int)pass, 100);

            Assert.AreEqual(OperationType.Ignore, entityWrapper.OperationType);
        }

        [TestMethod]
        public void ProcessEntityPassCreateZeroPassEntity()
        {
            Entity entity = new Entity(passOneReferences.First());
            EntityWrapper entityWrapper = new EntityWrapper(entity);

            PassType pass = PassType.CreateEntity;

            systemUnderTest.ProcessEntity(entityWrapper, (int)pass, 100);

            Assert.AreEqual(OperationType.Ignore, entityWrapper.OperationType);
        }

        [TestMethod]
        public void ProcessEntityPassCreateOtherBusinessEntity()
        {
            Entity entity = new Entity("some business entity");
            EntityWrapper entityWrapper = new EntityWrapper(entity);

            PassType pass = PassType.CreateEntity;

            systemUnderTest.ProcessEntity(entityWrapper, (int)pass, 100);

            Assert.AreEqual(OperationType.New, entityWrapper.OperationType);
        }

        [TestMethod]
        public void ProcessEntityOtherPassZeroPassEntity()
        {
            Entity entity = new Entity(passOneReferences.First());
            EntityWrapper entityWrapper = new EntityWrapper(entity)
            {
                OperationType = OperationType.Associate // some random to ensure not changed
            };
            PassType pass = PassType.SetRecordStatus; // some other further pass

            systemUnderTest.ProcessEntity(entityWrapper, (int)pass, 100);

            Assert.AreEqual(OperationType.Associate, entityWrapper.OperationType);
        }

        [TestMethod]
        public void ProcessEntityOtherpassOtherBusinessEntity()
        {
            Entity entity = new Entity("some business entity");
            EntityWrapper entityWrapper = new EntityWrapper(entity)
            {
                OperationType = OperationType.Associate // some random to ensure not changed
            };
            PassType pass = PassType.SetRecordStatus; // some other further pass

            systemUnderTest.ProcessEntity(entityWrapper, (int)pass, 100);

            Assert.AreEqual(OperationType.Associate, entityWrapper.OperationType);
        }
    }
}