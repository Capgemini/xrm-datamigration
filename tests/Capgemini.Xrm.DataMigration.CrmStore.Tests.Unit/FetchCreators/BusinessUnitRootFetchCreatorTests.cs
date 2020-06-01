using Capgemini.DataMigration.Core.Tests.Base;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.CrmStore.FetchCreators.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class BusinessUnitRootFetchCreatorTests : UnitTestBase
    {
        private BusinessUnitRootFetchCreator systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();

            systemUnderTest = new BusinessUnitRootFetchCreator();
        }

        [TestMethod]
        public void GetExportFetchXmlNullCrmField()
        {
            var actual = systemUnderTest.GetExportFetchXML("contact", null);

            actual.Should().BeEmpty();
        }

        [TestMethod]
        public void GetExportFetchXmlCrmFieldLookupTypeIsBussinessUnit()
        {
            Model.CrmField crmField = new Model.CrmField()
            {
                LookupType = "businessunit",
                FieldName = "contactbusinessunit"
            };
            var actual = systemUnderTest.GetExportFetchXML("contact", crmField);

            actual.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void GetExportFetchXmlCrmFieldLookupTypeIsNotBussinessUnit()
        {
            Model.CrmField crmField = new Model.CrmField()
            {
                LookupType = "account",
                FieldName = "contacaccount"
            };
            var actual = systemUnderTest.GetExportFetchXML("contact", crmField);

            actual.Should().BeEmpty();
        }

        [TestMethod]
        public void UseForEntity()
        {
            var actual = systemUnderTest.UseForEntity("TestEntity");

            actual.Should().Be(true);
        }
    }
}