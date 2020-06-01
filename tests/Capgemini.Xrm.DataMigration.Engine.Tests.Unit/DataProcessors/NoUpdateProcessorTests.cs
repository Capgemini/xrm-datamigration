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
    public class NoUpdateProcessorTests : UnitTestBase
    {
        private readonly List<string> noUpdateEntities = new List<string> { "calendar" };

        private NoUpdateProcessor systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
        }

        [TestMethod]
        public void NoUpdateProcessor()
        {
            FluentActions.Invoking(() => new NoUpdateProcessor(noUpdateEntities, MockLogger.Object))
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ImportCompleted()
        {
            systemUnderTest = new NoUpdateProcessor(noUpdateEntities, MockLogger.Object);

            FluentActions.Invoking(() => systemUnderTest.ImportCompleted())
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ImportStarted()
        {
            systemUnderTest = new NoUpdateProcessor(noUpdateEntities, MockLogger.Object);

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
            systemUnderTest = new NoUpdateProcessor(noUpdateEntities, MockLogger.Object);

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entity, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();
        }

        [DataTestMethod]
        [DataRow(PassType.CreateRequiredEntity)]
        [DataRow(PassType.CreateEntity)]
        public void ProcessEntityCreatePassesAndNoUpdateEntityNotIgnored(int pass)
        {
            systemUnderTest = new NoUpdateProcessor(noUpdateEntities, MockLogger.Object);
            var entity = new Entity(noUpdateEntities.First());
            var entityWrapper = new EntityWrapper(entity);

            systemUnderTest.ProcessEntity(entityWrapper, pass, 100);

            Assert.AreNotEqual(OperationType.Ignore, entityWrapper.OperationType);
        }

        [DataTestMethod]
        [DataRow(PassType.AssignRecord)]
        [DataRow(PassType.SetRecordStatus)]
        [DataRow(PassType.UpdateLookups)]
        public void ProcessEntityNonCreatePassesAndNoUpdateEntityIgnored(int pass)
        {
            systemUnderTest = new NoUpdateProcessor(noUpdateEntities, MockLogger.Object);
            var entity = new Entity(noUpdateEntities.First());
            var entityWrapper = new EntityWrapper(entity);

            systemUnderTest.ProcessEntity(entityWrapper, pass, 100);

            Assert.AreEqual(OperationType.Ignore, entityWrapper.OperationType);
        }

        [DataTestMethod]
        [DataRow(PassType.AssignRecord)]
        [DataRow(PassType.SetRecordStatus)]
        [DataRow(PassType.UpdateLookups)]
        public void ProcessEntityNonCreatePassesAndNotNoUpdateEntityNotIgnored(int pass)
        {
            systemUnderTest = new NoUpdateProcessor(noUpdateEntities, MockLogger.Object);

            var entity = new Entity("contact");
            var entityWrapper = new EntityWrapper(entity);

            systemUnderTest.ProcessEntity(entityWrapper, pass, 100);

            Assert.AreNotEqual(OperationType.Ignore, entityWrapper.OperationType);
        }
    }
}