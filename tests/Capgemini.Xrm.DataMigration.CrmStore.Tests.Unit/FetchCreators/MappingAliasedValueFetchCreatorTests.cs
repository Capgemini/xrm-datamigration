using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Capgemini.Xrm.DataMigration.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.CrmStore.FetchCreators.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class MappingAliasedValueFetchCreatorTests
    {
        private MappingAliasedValueFetchCreator systemUnderTest;

        [TestMethod]
        public void MappingAliasedValueFetchCreator()
        {
            Dictionary<string, Dictionary<string, List<string>>> lookupMappings = new Dictionary<string, Dictionary<string, List<string>>>();

            FluentActions.Invoking(() => new MappingAliasedValueFetchCreator(lookupMappings))
                    .Should()
                    .NotThrow();
        }

        [TestMethod]
        public void GetExportFetchXmlNullMappings()
        {
            var crmField = new CrmField()
            {
                LookupType = "account",
                FieldName = "contacaccount"
            };

            Dictionary<string, Dictionary<string, List<string>>> lookupMappings = null;

            systemUnderTest = new MappingAliasedValueFetchCreator(lookupMappings);

            FluentActions.Invoking(() => systemUnderTest.GetExportFetchXML("contact", crmField))
                .Should()
                .Throw<NullReferenceException>();
        }

        [TestMethod]
        public void GetExportFetchXmlEmptyMappings()
        {
            var crmField = new CrmField()
            {
                LookupType = "account",
                FieldName = "contacaccount"
            };

            Dictionary<string, Dictionary<string, List<string>>> lookupMappings = new Dictionary<string, Dictionary<string, List<string>>>();

            systemUnderTest = new MappingAliasedValueFetchCreator(lookupMappings);

            var actual = systemUnderTest.GetExportFetchXML("contact", crmField);

            actual.Should().BeEmpty();
        }

        [TestMethod]
        public void GetExportFetchXmlPrimaryKey()
        {
            var entityName = "account";
            var crmField = new CrmField()
            {
                LookupType = entityName,
                FieldName = "accountid",
                PrimaryKey = true
            };

            var list = new Dictionary<string, List<string>>
            {
                { "accountid", new List<string> { Guid.NewGuid().ToString() } }
            };
            var lookupMappings = new Dictionary<string, Dictionary<string, List<string>>>
            {
                { entityName, list }
            };

            systemUnderTest = new MappingAliasedValueFetchCreator(lookupMappings);

            var actual = systemUnderTest.GetExportFetchXML(entityName, crmField);

            actual.Should().NotBeEmpty();
        }

        [TestMethod]
        public void GetExportFetchXmlNonPrimaryKey()
        {
            var entityName = "account";
            var crmField = new CrmField()
            {
                LookupType = entityName,
                FieldName = "name"
            };

            var list = new Dictionary<string, List<string>>
            {
                { "name", new List<string> { "testname" } }
            };
            var lookupMappings = new Dictionary<string, Dictionary<string, List<string>>>
            {
                { entityName, list }
            };

            systemUnderTest = new MappingAliasedValueFetchCreator(lookupMappings);

            var actual = systemUnderTest.GetExportFetchXML(entityName, crmField);

            actual.Should().NotBeEmpty();
        }

        [TestMethod]
        public void UseForEntityNullLookupMappings()
        {
            Dictionary<string, Dictionary<string, List<string>>> lookupMappings = null;

            systemUnderTest = new MappingAliasedValueFetchCreator(lookupMappings);

            var actual = systemUnderTest.UseForEntity("TestEntity");

            actual.Should().Be(false);
        }

        [TestMethod]
        public void UseForEntity()
        {
            Dictionary<string, Dictionary<string, List<string>>> lookupMappings = new Dictionary<string, Dictionary<string, List<string>>>();

            systemUnderTest = new MappingAliasedValueFetchCreator(lookupMappings);

            var actual = systemUnderTest.UseForEntity("TestEntity");

            actual.Should().Be(false);
        }
    }
}