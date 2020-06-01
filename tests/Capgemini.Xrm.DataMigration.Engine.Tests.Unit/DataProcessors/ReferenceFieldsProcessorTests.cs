using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using Capgemini.Xrm.DataMigration.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Moq;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.DataProcessors
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ReferenceFieldsProcessorTests
    {
        private Mock<IEntityMetadataCache> mockEntityMetadataCache;
        private List<string> passOneReferences;

        private ReferenceFieldsProcessor systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            mockEntityMetadataCache = new Mock<IEntityMetadataCache>();
            passOneReferences = new List<string> { "passoneFirst", "passoneSecond" };

            systemUnderTest = new ReferenceFieldsProcessor(mockEntityMetadataCache.Object, passOneReferences);
        }

        [TestMethod]
        public void ReferenceFieldsProcessor()
        {
            FluentActions.Invoking(() => new ReferenceFieldsProcessor(mockEntityMetadataCache.Object, passOneReferences))
                 .Should()
                 .NotThrow();
        }

        [TestMethod]
        public void ProcessEntityAnnotation()
        {
            var entity = new EntityWrapper(new Entity("annotation", Guid.NewGuid()));

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entity, 1, 3))
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ProcessEntityUpdateLookups()
        {
            var entity = new EntityWrapper(new Entity("annotation", Guid.NewGuid()));

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entity, 2, 3))
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ProcessEntity()
        {
            var idAliasKey = "Blogs";

            var entity = new Entity("contact", Guid.NewGuid());
            entity.Attributes["firstname"] = "Joe";
            entity.Attributes["firstname"] = "Blogs";
            var entityWrapper = new EntityWrapper(entity);

            mockEntityMetadataCache.Setup(a => a.GetIdAliasKey(It.IsAny<string>())).Returns(idAliasKey);

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entityWrapper, 1, 3))
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ProcessEntityEntityReferenceAlias()
        {
            var idAliasKey = new EntityReference("account", "accountid", Guid.NewGuid());

            var entity = new Entity("contact", Guid.NewGuid());
            entity.Attributes["firstname"] = "Joe";
            entity.Attributes["firstname"] = "Blogs";
            entity.Attributes["account"] = idAliasKey;
            var entityWrapper = new EntityWrapper(entity);

            mockEntityMetadataCache.Setup(a => a.GetIdAliasKey(It.IsAny<string>())).Returns(idAliasKey.ToString());

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entityWrapper, 2, 3))
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ProcessEntityEntityReferenceAliasCreateEntity()
        {
            var idAliasKey = new EntityReference("account", "accountid", Guid.NewGuid());

            var entity = new Entity("contact", Guid.NewGuid());
            entity.Attributes["firstname"] = "Joe";
            entity.Attributes["firstname"] = "Blogs";
            entity.Attributes["account"] = idAliasKey;
            var entityWrapper = new EntityWrapper(entity);

            mockEntityMetadataCache.Setup(a => a.GetIdAliasKey(It.IsAny<string>())).Returns(idAliasKey.ToString());

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entityWrapper, 1, 3))
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
        public void ProcessEntityDonNotRemoveZeropassFields()
        {
            string textAttributeName = "some text field";
            string textAttributeValue = "some random value";

            Entity entity = new Entity(passOneReferences.First());
            entity.Attributes.Add(textAttributeName, textAttributeValue);
            EntityWrapper entityWrapper = new EntityWrapper(entity);

            PassType pass = PassType.CreateRequiredEntity;

            systemUnderTest.ProcessEntity(entityWrapper, (int)pass, 100);

            Assert.IsTrue(entity.Attributes.Contains(textAttributeName));
            Assert.AreEqual(textAttributeValue, entity.GetAttributeValue<string>(textAttributeName));
        }
    }
}