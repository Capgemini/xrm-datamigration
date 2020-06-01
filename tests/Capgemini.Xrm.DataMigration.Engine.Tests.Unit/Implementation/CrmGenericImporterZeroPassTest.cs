using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.CrmStore.DataStores;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Moq;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.Implementation
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmGenericImporterZeroPassTest : UnitTestBase
    {
        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
        }

        [TestMethod]
        public void ProcessZeroEntitiesFirst()
        {
            MockEntityRepo.SetupGet(p => p.GetEntityMetadataCache).Returns(MockEntityMetadataCache.Object);
            MockEntityMetadataCache.Setup(a => a.GetEntityMetadata(It.IsAny<string>())).Returns(new EntityMetadata());
            MockEntityMetadataCache.Setup(a => a.GetIdAliasKey(It.IsAny<string>())).Returns("testvalue");

            DataCrmStoreWriter dsw = new DataCrmStoreWriter(MockLogger.Object, MockEntityRepo.Object, 200, null);
            var storeReader = new Mock<IDataStoreReader<Entity, EntityWrapper>>();
            var config = new Mock<ICrmGenericImporterConfig>();

            // setup pass zero entities
            config.SetupGet(c => c.PassOneReferences).Returns(new List<string> { "businessunit", "uom", "uomschedule", "queue" });

            // handle data store reader - after rested put entities back into queue
            Queue<List<EntityWrapper>> queue = GetMockedData();
            storeReader.Setup(sr => sr.ReadBatchDataFromStore()).Returns(queue.Dequeue);
            storeReader.Setup(sr => sr.Reset()).Callback(() => GetMockedData().All(data =>
            {
                queue.Enqueue(data);
                return true;
            })); // reset the queue!

            // record order of saving entites
            List<EntityWrapper> actual = new List<EntityWrapper>();
            MockEntityRepo.Setup(repo => repo.CreateUpdateEntities(It.IsAny<List<EntityWrapper>>())).Callback<List<EntityWrapper>>(list => actual.AddRange(list));

            // execute test
            TestCrmGenericImporter importer = new TestCrmGenericImporter(MockLogger.Object, storeReader.Object, dsw, config.Object);
            importer.MigrateData();

            // 3 batches - 3 calls!
            MockEntityRepo.Verify(repo => repo.CreateUpdateEntities(It.IsAny<List<EntityWrapper>>()), Times.Exactly(3));

            MockEntityMetadataCache.Verify(a => a.GetEntityMetadata(It.IsAny<string>()));
            MockEntityMetadataCache.Verify(a => a.GetIdAliasKey(It.IsAny<string>()));

            // queue should be added first
            Assert.AreEqual("queue", actual[0].OriginalEntity.LogicalName);
            Assert.AreEqual("queue", actual[1].OriginalEntity.LogicalName);
        }

        [TestMethod]
        public void GetStartingPassNumberWithPassZeroEntities()
        {
            DataCrmStoreWriter dsw = new DataCrmStoreWriter(MockLogger.Object, MockEntityRepo.Object, 200, null);
            Mock<IDataStoreReader<Entity, EntityWrapper>> storeReader = new Mock<IDataStoreReader<Entity, EntityWrapper>>();
            Mock<ICrmGenericImporterConfig> config = new Mock<ICrmGenericImporterConfig>();

            // setup pass zero entities
            config.SetupGet(c => c.PassOneReferences).Returns(new List<string> { "businessunit", "uom", "uomschedule", "queue" });

            // execute test
            TestCrmGenericImporter importer = new TestCrmGenericImporter(MockLogger.Object, storeReader.Object, dsw, config.Object);
            var actual = importer.GetStartingPassNumber();

            Assert.AreEqual((int)PassType.CreateRequiredEntity, actual, "Ivalid starting pass number");
        }

        [TestMethod]
        public void GetStartingPassNumberNoPassZeroEntities()
        {
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IEntityRepository> entityrepo = new Mock<IEntityRepository>();
            DataCrmStoreWriter dsw = new DataCrmStoreWriter(logger.Object, entityrepo.Object, 200, null);
            Mock<IDataStoreReader<Entity, EntityWrapper>> storeReader = new Mock<IDataStoreReader<Entity, EntityWrapper>>();
            Mock<ICrmGenericImporterConfig> config = new Mock<ICrmGenericImporterConfig>();

            // setup pass zero entities
            config.SetupGet(c => c.PassOneReferences).Returns(new List<string> { });

            // execute test
            TestCrmGenericImporter importer = new TestCrmGenericImporter(logger.Object, storeReader.Object, dsw, config.Object);
            var actual = importer.GetStartingPassNumber();

            Assert.AreEqual((int)PassType.CreateEntity, actual, "Ivalid starting pass number");
        }

        private static Queue<List<EntityWrapper>> GetMockedData()
        {
            return new Queue<List<EntityWrapper>>(
            new List<EntityWrapper>[]
            {
                Enumerable.Range(0, 10).Select(i => new EntityWrapper(new Entity("business entity with first leter smaller that queue!", Guid.NewGuid()) { Attributes = new AttributeCollection() { new KeyValuePair<string, object>("name", "some name!") } })).ToList(),
                Enumerable.Range(0, 2).Select(i => new EntityWrapper(new Entity("queue", Guid.NewGuid()) { Attributes = new AttributeCollection() { new KeyValuePair<string, object>("name", "some name!") } })).ToList(),
                Enumerable.Range(0, 5).Select(i => new EntityWrapper(new Entity("team", Guid.NewGuid()) { Attributes = new AttributeCollection() { new KeyValuePair<string, object>("name", "some name!") } })).ToList(),
                new List<EntityWrapper>()
            });
        }
    }
}