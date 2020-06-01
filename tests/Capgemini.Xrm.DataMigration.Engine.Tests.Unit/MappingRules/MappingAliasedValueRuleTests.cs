using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Engine.MappingRules;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

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
    }
}