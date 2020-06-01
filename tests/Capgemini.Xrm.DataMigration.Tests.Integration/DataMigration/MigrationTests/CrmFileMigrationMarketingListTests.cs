using System.Collections.Generic;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.IntegrationTests.DataMigration.MigrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.IntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmFileMigrationMarketingListTests : CrmFileMigrationBaseTest
    {
        public CrmFileMigrationMarketingListTests()
            : base(
            "TestData\\ImportSchemas\\MarketingList",
            "MarketingListDataSchema.xml",
            "MarketingList",
            ConnectionHelper.GetOrganizationalServiceSource(),
            ConnectionHelper.GetOrganizationalServiceTarget())
        {
        }

        protected override CrmImportConfig GetImporterConfig()
        {
            var importConfig = base.GetImporterConfig();
            importConfig.NoUpsertEntities.AddRange(new List<string>() { "list" });
            return importConfig;
        }
    }
}