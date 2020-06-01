using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.IntegrationTests.DataMigration.MigrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmFileMigrationMultiOptionSetTests : CrmFileMigrationBaseTest
    {
        public CrmFileMigrationMultiOptionSetTests()
            : base(
            "TestData\\ImportSchemas\\MultiOptionSet",
            "multioptionsetschema.xml",
            "multiselect",
            ConnectionHelper.GetOrganizationalServiceSource(),
            ConnectionHelper.GetOrganizationalServiceTarget(),
            true)
        {
        }
    }
}