using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.IntegrationTests.DataMigration.MigrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmFileMigrationAppointmentsTest : CrmFileMigrationBaseTest
    {
        public CrmFileMigrationAppointmentsTest()
            : base(
            "TestData\\ImportSchemas\\AppointmentsSchema",
            "apointmentsSchema.xml",
            "Appointments",
            ConnectionHelper.GetOrganizationalServiceSource(),
            ConnectionHelper.GetOrganizationalServiceTarget())
        {
        }
    }
}