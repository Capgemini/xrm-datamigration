using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Engine.MappingRules;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Moq;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.MappingRules
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class MappingAliasedValueRuleTests : UnitTestBase
    {
        private MappingAliasedValueRule systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();

            systemUnderTest = new MappingAliasedValueRule(MockEntityRepo.Object);
        }

        [TestMethod]
        public void MappingAliasedValueRule()
        {
            FluentActions.Invoking(() => new MappingAliasedValueRule(MockEntityRepo.Object))
                 .Should()
                 .NotThrow();
        }

        [TestMethod]
        public void ProcessImport()
        {
            string aliasedAttributeName = "name";
            List<AliasedValue> values = new List<AliasedValue>()
            {
             new AliasedValue("account", "accountid", "12345"),
             new AliasedValue("account", "name", "Test Account"),
             new AliasedValue("account", "employeecount", "1000"),
            };

            var actual = systemUnderTest.ProcessImport(aliasedAttributeName, values, out object replacementValue);

            actual.Should().BeFalse();
            replacementValue.Should().NotBeNull();
        }

        [TestMethod]
        public void ProcessImportDuplicateAliasMappingQueriesAreCached()
        {
            string aliasedAttributeName = "accountid";
            var firstValues = new List<AliasedValue>()
            {
                new AliasedValue("account", "name", "Test Account"),
            };
            var secondValues = new List<AliasedValue>()
            {
                new AliasedValue("account", "name", "Test Account"),
            };
            var returnedGuid = Guid.NewGuid();
            MockEntityRepo
                .Setup(r => r.GetGuidForMapping(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<object[]>()))
                .Returns(returnedGuid);

            systemUnderTest.ProcessImport(aliasedAttributeName, firstValues, out object replacementValue);
            systemUnderTest.ProcessImport(aliasedAttributeName, secondValues, out replacementValue);

            MockEntityRepo.Verify(
                r => r.GetGuidForMapping(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<object[]>()),
                Times.AtMostOnce(),
                "Expected alias mappings to be cached but they were not.");

            replacementValue.Should().BeEquivalentTo(returnedGuid);
        }

        [TestMethod]
        public void ProcessImportEmptyGuidsAreNotCached()
        {
            string aliasedAttributeName = "accountid";
            var firstValues = new List<AliasedValue>()
            {
                new AliasedValue("account", "name", "Test Account"),
            };
            var secondValues = new List<AliasedValue>()
            {
                new AliasedValue("account", "name", "Test Account"),
            };
            MockEntityRepo
                .Setup(r => r.GetGuidForMapping(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<object[]>()))
                .Returns(Guid.Empty);

            systemUnderTest.ProcessImport(aliasedAttributeName, firstValues, out object replacementValue);
            systemUnderTest.ProcessImport(aliasedAttributeName, secondValues, out replacementValue);

            MockEntityRepo.Verify(
                r => r.GetGuidForMapping(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<object[]>()),
                Times.Exactly(2),
                "Expected empty GUIDs not to be cached but they were.");

            replacementValue.Should().BeEquivalentTo(Guid.Empty);
        }
    }
}