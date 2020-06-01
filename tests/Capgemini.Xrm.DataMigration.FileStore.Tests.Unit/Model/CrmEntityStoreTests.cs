using System;
using System.Diagnostics.CodeAnalysis;
using Capgemini.Xrm.DataMigration.DataStore;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.FileStore.Model.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmEntityStoreTests
    {
        private CrmEntityStore systemUnderTest = null;

        [TestMethod]
        public void CrmEntityStoreTest()
        {
            FluentActions.Invoking(() => systemUnderTest = new CrmEntityStore())
                           .Should()
                           .NotThrow();

            Assert.IsTrue(systemUnderTest.Attributes.Count == 0);
        }

        [TestMethod]
        public void CrmEntityStoreNullEntity()
        {
            Entity entity = null;

            FluentActions.Invoking(() => systemUnderTest = new CrmEntityStore(entity))
                           .Should()
                           .Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CrmEntityStoreEntity()
        {
            var entity = new Entity("test", Guid.NewGuid());

            FluentActions.Invoking(() => systemUnderTest = new CrmEntityStore(entity))
                           .Should()
                           .NotThrow();

            Assert.IsTrue(systemUnderTest.Attributes.Count == 0);
            Assert.AreEqual(entity.Id, systemUnderTest.Id);
            Assert.AreEqual(entity.LogicalName, systemUnderTest.LogicalName);
            Assert.AreEqual(entity.Attributes.Count, systemUnderTest.Attributes.Count);
        }

        [TestMethod]
        public void CrmEntityStoreNullEntityWrapper()
        {
            EntityWrapper entity = null;

            FluentActions.Invoking(() => systemUnderTest = new CrmEntityStore(entity))
                           .Should()
                           .Throw<ArgumentNullException>();
        }
    }
}