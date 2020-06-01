using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Moq;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.DataProcessors
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class SyncEntitiesProcessorTests : UnitTestBase
    {
        private List<string> entitiesToSync;

        private SyncEntitiesProcessor systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();

            entitiesToSync = new List<string>
            {
                "contact"
            };
        }

        [TestMethod]
        public void SyncEntitiesProcessor()
        {
            FluentActions.Invoking(() => new SyncEntitiesProcessor(entitiesToSync, MockEntityRepo.Object, MockLogger.Object))
                   .Should()
                   .NotThrow();
        }

        [TestMethod]
        public void ImportCompleted()
        {
            systemUnderTest = new SyncEntitiesProcessor(entitiesToSync, MockEntityRepo.Object, MockLogger.Object);

            var entity = new EntityWrapper(new Entity("contact", Guid.NewGuid()));
            var currentEntities = new List<Entity> { new Entity("account", Guid.NewGuid()) };
            MockEntityRepo.Setup(a => a.GetEntitiesByName(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<int>()))
                .Returns(currentEntities);

            systemUnderTest.ProcessEntity(entity, 1, 1);
            systemUnderTest.ProcessEntity(new EntityWrapper(new Entity("contact", Guid.NewGuid())), 1, 1);
            systemUnderTest.ProcessEntity(new EntityWrapper(new Entity("account", Guid.NewGuid())), 1, 1);
            systemUnderTest.ProcessEntity(new EntityWrapper(new Entity("opportunity", Guid.NewGuid())), 1, 1);

            FluentActions.Invoking(() => systemUnderTest.ImportCompleted())
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ImportStarted()
        {
            systemUnderTest = new SyncEntitiesProcessor(entitiesToSync, MockEntityRepo.Object, MockLogger.Object);

            FluentActions.Invoking(() => systemUnderTest.ImportStarted())
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ProcessEntity()
        {
            systemUnderTest = new SyncEntitiesProcessor(entitiesToSync, MockEntityRepo.Object, MockLogger.Object);

            var entity = new EntityWrapper(new Entity("contact", Guid.NewGuid()));
            int passNumber = 1;
            int maxPassNumber = 1;

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entity, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();
        }
    }
}