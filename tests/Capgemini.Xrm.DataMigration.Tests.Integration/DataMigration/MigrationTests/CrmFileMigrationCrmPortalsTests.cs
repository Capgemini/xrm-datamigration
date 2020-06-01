using System.Collections.Generic;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.IntegrationTests.DataMigration.MigrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.IntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmFileMigrationCrmPortalsTests : CrmFileMigrationBaseTest
    {
        public CrmFileMigrationCrmPortalsTests()
            : base(
            "TestData\\ImportSchemas\\CrmPortal",
            "CrmPortalFullSchemaWithNotes.xml",
            "CrmPortalsData",
            ConnectionHelper.GetOrganizationalServicePortalSource(),
            ConnectionHelper.GetOrganizationalServicePortalTarget())
        {
        }

        protected override CrmImportConfig GetImporterConfig()
        {
            var importConfig = base.GetImporterConfig();

            importConfig.EntitiesToSync.AddRange(new List<string>() { "adx_weblink", "adx_webpage" });

            return importConfig;
        }
    }
}