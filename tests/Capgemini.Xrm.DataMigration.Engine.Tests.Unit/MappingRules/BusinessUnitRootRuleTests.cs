using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Capgemini.Xrm.DataMigration.Engine.MappingRules;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.MappingRules
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class BusinessUnitRootRuleTests
    {
        private BusinessUnitRootRule systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            systemUnderTest = new BusinessUnitRootRule(Guid.NewGuid());
        }

        [TestMethod]
        public void BusinessUnitRootRule()
        {
            FluentActions.Invoking(() => new BusinessUnitRootRule(Guid.NewGuid()))
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
            replacementValue.Should().BeNull();
        }
    }
}