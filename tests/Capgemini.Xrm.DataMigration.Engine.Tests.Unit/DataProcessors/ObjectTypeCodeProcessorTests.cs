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
    public class ObjectTypeCodeProcessorTests : UnitTestBase
    {
        private readonly ObjectTypeCodeMappingConfiguration objectTypeCodeMappingConfig = new ObjectTypeCodeMappingConfiguration();

        private ObjectTypeCodeProcessor systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
        }

        [TestMethod]
        public void ObjectTypeCodeProcessor()
        {
            FluentActions.Invoking(() => new ObjectTypeCodeProcessor(objectTypeCodeMappingConfig, MockLogger.Object, MockEntityRepo.Object))
                .Should()
                .NotThrow();
        }

        [TestMethod]
        public void ImportCompleted()
        {
            systemUnderTest = new ObjectTypeCodeProcessor(objectTypeCodeMappingConfig, MockLogger.Object, MockEntityRepo.Object);

            FluentActions.Invoking(() => systemUnderTest.ImportCompleted())
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ImportStarted()
        {
            systemUnderTest = new ObjectTypeCodeProcessor(objectTypeCodeMappingConfig, MockLogger.Object, MockEntityRepo.Object);

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

            systemUnderTest = new ObjectTypeCodeProcessor(objectTypeCodeMappingConfig, MockLogger.Object, MockEntityRepo.Object);

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entity, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ProcessEntityEntityWhichDoesntHaveFieldsToMapShouldNotUpdateTheEntitiesAttributes()
        {
            ObjectTypeCodeMappingConfiguration config = CreateConfiguration("cap_Entity", 10113, "cap_testfield");

            systemUnderTest = new ObjectTypeCodeProcessor(config, MockLogger.Object, MockEntityRepo.Object);

            string expectedFieldName = "cap_name";
            string expectedFieldValue = "TestEntity";
            Entity entity = new Entity("cap_testentity");
            entity.Attributes.Add(expectedFieldName, expectedFieldValue);
            EntityWrapper entityWrapper = new EntityWrapper(entity);

            systemUnderTest.ProcessEntity(entityWrapper, 0, 1);

            Assert.AreEqual(expectedFieldValue, entityWrapper.OriginalEntity[expectedFieldName]);
        }

        [TestMethod]
        public void ProcessEntityEntityWhichHasMapFieldButDoesntHaveOneOfMapValuesShouldNotUpdateTheEntitiesAttributes()
        {
            ObjectTypeCodeMappingConfiguration config = CreateConfiguration("cap_testentity", 10113, "cap_testfield");

            systemUnderTest = new ObjectTypeCodeProcessor(config, MockLogger.Object, MockEntityRepo.Object);

            string expectedFieldName = "cap_testfield";
            int expectedFieldValue = 10222;
            Entity entity = new Entity("cap_testentity");
            entity.Attributes.Add(expectedFieldName, expectedFieldValue);
            EntityWrapper entityWrapper = new EntityWrapper(entity);

            systemUnderTest.ProcessEntity(entityWrapper, 0, 1);

            Assert.AreEqual(expectedFieldValue, entityWrapper.OriginalEntity[expectedFieldName]);
        }

        [TestMethod]
        public void ProcessEntityEntityWhichHasFieldToMapShouldCallTheRepoToGetEntityTypeCode()
        {
            string expectedEntity = "cap_testentity";
            string expectedFieldName = "cap_testfield";
            int expectedFieldValue = 10113;

            var config = CreateConfiguration(expectedEntity, 10113, expectedFieldName);

            systemUnderTest = new ObjectTypeCodeProcessor(config, MockLogger.Object, MockEntityRepo.Object);

            Entity entity = new Entity(expectedEntity);
            entity.Attributes.Add(expectedFieldName, expectedFieldValue);
            EntityWrapper entityWrapper = new EntityWrapper(entity);
            InitRepoToReturnMetaData(expectedEntity);

            systemUnderTest.ProcessEntity(entityWrapper, 0, 1);

            MockEntityRepo.Verify(repo => repo.GetEntityMetadataCache.GetEntityMetadata(expectedEntity), Times.Once);
        }

        private void InitRepoToReturnMetaData(string entityLogicalName)
        {
            MockEntityRepo.SetupGet(r => r.GetEntityMetadataCache)
                .Returns(MockEntityMetadataCache.Object);

            MockEntityRepo.Setup(r => r.GetEntityMetadataCache.GetEntityMetadata(entityLogicalName)).Returns<string>(entityName =>
            {
                EntityMetadata metaData = new EntityMetadata();
                return metaData;
            });
        }

        private ObjectTypeCodeMappingConfiguration CreateConfiguration(string entityName, int typeCode, string fieldName)
        {
            var config = new ObjectTypeCodeMappingConfiguration();
            config.EntityToTypeCodeMapping.Add(entityName, typeCode);
            config.FieldsToSearchForMapping.AddRange(new List<string>() { fieldName });

            return config;
        }
    }
}