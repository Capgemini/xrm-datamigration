using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Capgemini.Xrm.DataMigration.Cache;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Core.IntegrationTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class EntityMetadataCacheTest
    {
        private readonly string entityName = "contact";
        private readonly string attributeName = "contactid";

        private EntityMetadataCache systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            systemUnderTest = new EntityMetadataCache(ConnectionHelper.GetOrganizationalServiceSource());
        }

        [TestMethod]
        public void GetFromEntityMetadata()
        {
            var entItem = systemUnderTest.GetEntityMetadata("contact");
            var actual = entItem.Attributes.Select(p => p.AttributeType).ToList();

            actual.Should().NotBeNull();
        }

        [TestMethod]
        public void GetContactIdFromEntityMetadata()
        {
            var actual = systemUnderTest.GetAttributeDotNetType("contact", "contactid");

            Assert.AreEqual(typeof(Guid), actual);
        }

        [TestMethod]
        public void GetUserSettingsFromEntityMetadata()
        {
            var actual = systemUnderTest.GetEntityMetadata("usersettings");

            actual.Should().NotBeNull();
        }

        [TestMethod]
        public void GetSystemUserFromEntityMetadata()
        {
            var actual = systemUnderTest.GetAttributeDotNetType("usersettings", "systemuserid");

            Assert.AreEqual(typeof(Guid), actual);
        }

        [TestMethod]
        public void GetBusinessUnitFromEntityMetadata()
        {
            var actual = systemUnderTest.GetAttributeDotNetType("systemuser", "businessunitid");

            Assert.AreEqual(typeof(EntityReference), actual);
        }

        [TestMethod]
        public void LookupFieldTest()
        {
            var actual = systemUnderTest.GetLookUpEntityName("systemuser", "businessunitid");

            actual.Should().Be("businessunit");
        }

        [TestMethod]
        public void GetEntityMetadata()
        {
            FluentActions.Invoking(() => systemUnderTest.GetEntityMetadata(entityName))
                        .Should()
                        .NotThrow();
        }

        [TestMethod]
        public void GetLookUpEntityName()
        {
            FluentActions.Invoking(() => systemUnderTest.GetLookUpEntityName(entityName, attributeName))
                        .Should()
                        .NotThrow();
        }

        [TestMethod]
        public void GetIdAliasKey()
        {
            FluentActions.Invoking(() => systemUnderTest.GetIdAliasKey(entityName))
                        .Should()
                        .NotThrow();
        }
    }
}