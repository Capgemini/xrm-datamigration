using System;
using System.Collections.Generic;
using Capgemini.DataMigration.Core;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Moq;

namespace Capgemini.Xrm.DataMigration.IntegrationTests.ProcessorsUnitTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class DeleteEntitiesProcessorTest
    {
        [TestMethod]
        public void TestWithoutDelete()
        {
            List<string> entToDel = new List<string>() { "test", "test1" };
            Guid id = Guid.NewGuid();
            Mock<IEntityRepository> entRepoMock = new Mock<IEntityRepository>();
            entRepoMock.Setup(p => p.GetEntitiesByName("test", It.IsAny<string[]>(), It.IsAny<int>())).Returns(new List<Entity> { new Entity("test", id) });

            IEntityRepository entRepo = entRepoMock.Object;

            SyncEntitiesProcessor entProc = new SyncEntitiesProcessor(entToDel, entRepo, new ConsoleLogger());
            entProc.ImportStarted();
            Entity ent = new Entity("test", id);
            EntityWrapper entWrap = new EntityWrapper(ent, true);
            entProc.ProcessEntity(entWrap, 3, 3);

            entProc.ImportCompleted();

            entRepoMock.Verify(p => p.GetEntitiesByName("test", It.IsAny<string[]>(), It.IsAny<int>()), Times.Exactly(1));
            entRepoMock.Verify(p => p.DeleteEntity(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
            Assert.AreEqual(entWrap.OperationType, OperationType.Ignore);
        }

        [TestMethod]
        public void TestWitDelete()
        {
            List<string> entToDel = new List<string>() { "test", "test1" };

            Guid id = Guid.NewGuid();
            Guid exId = Guid.NewGuid();
            Mock<IEntityRepository> entRepoMock = new Mock<IEntityRepository>();
            entRepoMock.Setup(p => p.GetEntitiesByName("test", It.IsAny<string[]>(), It.IsAny<int>())).Returns(new List<Entity> { new Entity("test", id), new Entity("test", exId), new Entity("test", Guid.NewGuid()) });
            IEntityRepository entRepo = entRepoMock.Object;
            SyncEntitiesProcessor entProc = new SyncEntitiesProcessor(entToDel, entRepo, new ConsoleLogger());

            entProc.ImportStarted();

            Entity ent = new Entity("test", id);
            EntityWrapper entWrap = new EntityWrapper(ent, true);
            entProc.ProcessEntity(entWrap, 3, 3);
            entProc.ImportCompleted();

            entRepoMock.Verify(p => p.GetEntitiesByName("test", It.IsAny<string[]>(), It.IsAny<int>()), Times.Exactly(1));
            entRepoMock.Verify(p => p.DeleteEntity("test", id), Times.Never);
            entRepoMock.Verify(p => p.DeleteEntity("test", exId), Times.Exactly(1));
            entRepoMock.Verify(p => p.DeleteEntity("test", It.IsAny<Guid>()), Times.Exactly(2));
            Assert.AreEqual(entWrap.OperationType, OperationType.Ignore);
        }

        [TestMethod]
        public void TestWithDeleteNoM2M()
        {
            List<string> entToDel = new List<string>() { "test", "test1" };

            Guid id = Guid.NewGuid();
            Guid exId = Guid.NewGuid();
            Mock<IEntityRepository> entRepoMock = new Mock<IEntityRepository>();
            entRepoMock.Setup(p => p.GetEntitiesByName("test", It.IsAny<string[]>(), It.IsAny<int>())).Returns(new List<Entity> { new Entity("test", id), new Entity("test", exId), new Entity("test", Guid.NewGuid()) });
            IEntityRepository entRepo = entRepoMock.Object;
            SyncEntitiesProcessor entProc = new SyncEntitiesProcessor(entToDel, entRepo, new ConsoleLogger());

            entProc.ImportStarted();

            Entity ent = new Entity("test", id);
            EntityWrapper entWrap = new EntityWrapper(ent);
            entProc.ProcessEntity(entWrap, 3, 3);
            entProc.ImportCompleted();

            entRepoMock.Verify(p => p.GetEntitiesByName("test", It.IsAny<string[]>(), It.IsAny<int>()), Times.Exactly(1));
            entRepoMock.Verify(p => p.DeleteEntity("test", exId), Times.Exactly(1));
            Assert.AreEqual(entWrap.OperationType, OperationType.Ignore);
        }
    }
}