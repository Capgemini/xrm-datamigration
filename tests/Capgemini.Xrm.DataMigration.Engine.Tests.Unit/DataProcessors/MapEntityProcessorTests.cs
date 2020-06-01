using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Moq;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.DataProcessors
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class MapEntityProcessorTests : UnitTestBase
    {
        private readonly int passNumber = 1;
        private readonly int maxPassNumber = 3;
        private readonly MappingConfiguration mappingConfig = new MappingConfiguration();
        private readonly List<string> passOneReferences = new List<string>();
        private string entityName = "account";

        private MapEntityProcessor systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
        }

        [TestMethod]
        public void MappEntityProcessorNullMappingConfiguration()
        {
            FluentActions.Invoking(() => systemUnderTest = new MapEntityProcessor(null, MockLogger.Object, MockEntityRepo.Object, passOneReferences))
                            .Should()
                            .NotThrow();

            MockEntityRepo.Verify(a => a.GetParentBuId());
            MockEntityRepo.Verify(a => a.GetOrganizationId());
            MockEntityRepo.Verify(a => a.GetEntityMetadataCache);
        }

        [TestMethod]
        public void MappEntityProcessor()
        {
            FluentActions.Invoking(() => new MapEntityProcessor(mappingConfig, MockLogger.Object, MockEntityRepo.Object, passOneReferences))
                            .Should()
                            .NotThrow();

            MockEntityRepo.Verify(a => a.GetParentBuId());
            MockEntityRepo.Verify(a => a.GetOrganizationId());
            MockEntityRepo.Verify(a => a.GetEntityMetadataCache);
        }

        [TestMethod]
        public void MappEntityProcessorNullEntityRepository()
        {
            FluentActions.Invoking(() => new MapEntityProcessor(mappingConfig, MockLogger.Object, null, passOneReferences))
                            .Should()
                            .NotThrow();

            MockEntityRepo.Verify(a => a.GetParentBuId(), Times.Never);
            MockEntityRepo.Verify(a => a.GetOrganizationId(), Times.Never);
            MockEntityRepo.Verify(a => a.GetEntityMetadataCache, Times.Never);
        }

        [TestMethod]
        public void MappEntityProcessorWithEntityRepositoryNotNull()
        {
            MockEntityRepo.Setup(a => a.GetParentBuId()).Returns(Guid.NewGuid());

            FluentActions.Invoking(() => new MapEntityProcessor(mappingConfig, MockLogger.Object, MockEntityRepo.Object, passOneReferences))
                            .Should()
                            .NotThrow();

            MockEntityRepo.Verify(a => a.GetParentBuId());
            MockEntityRepo.Verify(a => a.GetOrganizationId());
            MockEntityRepo.Verify(a => a.GetEntityMetadataCache);
        }

        [TestMethod]
        public void ProcessEntityNullEntityWrapper()
        {
            EntityWrapper entity = null;

            systemUnderTest = new MapEntityProcessor(mappingConfig, MockLogger.Object, MockEntityRepo.Object, passOneReferences);

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entity, passNumber, maxPassNumber))
                         .Should()
                         .Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ProcessEntity()
        {
            EntityWrapper entity = new EntityWrapper(new Entity() { LogicalName = "testentity" });
            var entityMetadata = new EntityMetadata();

            MockEntityRepo.SetupGet(a => a.GetEntityMetadataCache).Returns(MockEntityMetadataCache.Object);
            MockEntityRepo.Setup(a => a.GetParentBuId()).Returns(Guid.NewGuid());
            MockEntityRepo.Setup(a => a.GetOrganizationId()).Returns(Guid.NewGuid());

            MockEntityMetadataCache.Setup(a => a.GetEntityMetadata(It.IsAny<string>())).Returns(entityMetadata);

            systemUnderTest = new MapEntityProcessor(mappingConfig, MockLogger.Object, MockEntityRepo.Object, passOneReferences);

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entity, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();

            MockEntityRepo.VerifyAll();
            MockEntityMetadataCache.Verify(a => a.GetEntityMetadata(It.IsAny<string>()));
        }

        [TestMethod]
        public void ProcessEntityOrganizationEnity()
        {
            var entity = new Entity() { LogicalName = "organization" };
            EntityWrapper entityWrapper = new EntityWrapper(entity);
            var entityMetadata = new EntityMetadata();

            MockEntityRepo.SetupGet(a => a.GetEntityMetadataCache).Returns(MockEntityMetadataCache.Object);
            MockEntityRepo.Setup(a => a.GetParentBuId()).Returns(Guid.NewGuid());
            MockEntityRepo.Setup(a => a.GetOrganizationId()).Returns(Guid.NewGuid());

            MockEntityMetadataCache.Setup(a => a.GetEntityMetadata(It.IsAny<string>())).Returns(entityMetadata);

            var values = new Dictionary<Guid, Guid>
            {
                { Guid.NewGuid(), Guid.NewGuid() },
                { Guid.NewGuid(), Guid.NewGuid() }
            };

            mappingConfig.Mappings.Add("buId", values);

            systemUnderTest = new MapEntityProcessor(mappingConfig, MockLogger.Object, MockEntityRepo.Object, passOneReferences);

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entityWrapper, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();

            MockEntityRepo.VerifyAll();
            MockEntityMetadataCache.Verify(a => a.GetEntityMetadata(It.IsAny<string>()));
        }

        [TestMethod]
        public void ProcessEntityManyToManyEnity()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var entity = new Entity() { LogicalName = "accountcontact" };
            entity.Attributes["accountid"] = id1;
            entity.Attributes["contactid"] = id2;
            var entityWrapper = new EntityWrapper(entity, true);

            var entityMetadata = new EntityMetadata();

            MockEntityRepo.SetupGet(a => a.GetEntityMetadataCache).Returns(MockEntityMetadataCache.Object);
            MockEntityRepo.Setup(a => a.GetParentBuId()).Returns(Guid.NewGuid());
            MockEntityRepo.Setup(a => a.GetOrganizationId()).Returns(Guid.NewGuid());

            MockEntityMetadataCache.Setup(a => a.GetEntityMetadata(It.IsAny<string>())).Returns(entityMetadata);

            var values = new Dictionary<Guid, Guid>
            {
                { id1, Guid.NewGuid() },
                { id2, Guid.NewGuid() }
            };

            mappingConfig.Mappings.Add("accountcontact", values);

            systemUnderTest = new MapEntityProcessor(mappingConfig, MockLogger.Object, MockEntityRepo.Object, passOneReferences);

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entityWrapper, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();

            MockEntityRepo.VerifyAll();
            MockEntityMetadataCache.Verify(a => a.GetEntityMetadata(It.IsAny<string>()));
        }

        [TestMethod]
        public void ProcessEntityApplyAliasMapping()
        {
            entityName = "ProcessEntityApplyAliasMapping";
            string lookUpName = "contact";
            string attributeName = "c.contactid";
            var testEntity = new Entity("contact", Guid.NewGuid());
            var listOfEntity = new List<Entity> { testEntity };

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.Uniqueidentifier);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            var oneToManyRelationshipMetadata = new OneToManyRelationshipMetadata
            {
                ReferencingAttribute = attributeName
            };
            var oneToManyRelationships = new List<OneToManyRelationshipMetadata>
            {
                oneToManyRelationshipMetadata
            };

            var entityMetaData = InitializeEntityMetadata(attributes, oneToManyRelationships);
            SetFieldValue(entityMetaData, "_primaryIdAttribute", attributeName);

            mappingConfig.ApplyAliasMapping = true;
            EntityWrapper entityWraper = new EntityWrapper(new Entity() { LogicalName = entityName });

            entityWraper.OriginalEntity.Attributes[attributeName] = new AliasedValue("contact", "contactid", testEntity.Id.ToString());

            MockEntityRepo.SetupGet(a => a.GetEntityMetadataCache).Returns(MockEntityMetadataCache.Object);
            MockEntityRepo.Setup(a => a.GetParentBuId()).Returns(Guid.NewGuid());
            MockEntityRepo.Setup(a => a.GetOrganizationId()).Returns(Guid.NewGuid());
            MockEntityRepo.Setup(a => a.FindEntitiesByName(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(listOfEntity);

            MockEntityMetadataCache.Setup(a => a.GetEntityMetadata(It.IsAny<string>())).Returns(entityMetaData);
            MockEntityMetadataCache.Setup(a => a.GetIdAliasKey(It.IsAny<string>())).Returns(attributeName);
            MockEntityMetadataCache.Setup(a => a.GetLookUpEntityName(It.IsAny<string>(), It.IsAny<string>()))
                                   .Returns(lookUpName);

            systemUnderTest = new MapEntityProcessor(mappingConfig, MockLogger.Object, MockEntityRepo.Object, passOneReferences);

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entityWraper, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();

            MockEntityRepo.VerifyAll();
            MockEntityMetadataCache.VerifyAll();
        }

        [TestMethod]
        public void ImportCompleted()
        {
            systemUnderTest = new MapEntityProcessor(mappingConfig, MockLogger.Object, MockEntityRepo.Object, passOneReferences);

            FluentActions.Invoking(() => systemUnderTest.ImportCompleted())
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ImportStarted()
        {
            systemUnderTest = new MapEntityProcessor(mappingConfig, MockLogger.Object, MockEntityRepo.Object, passOneReferences);

            FluentActions.Invoking(() => systemUnderTest.ImportStarted())
                         .Should()
                         .NotThrow();
        }
    }
}