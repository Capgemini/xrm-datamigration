using System.Collections.Generic;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.IntegrationTests.DataMigration.MigrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.IntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmFileMigrationServiceEndpointTests : CrmFileMigrationBaseTest
    {
        public CrmFileMigrationServiceEndpointTests()
            : base(
            "TestData\\ImportSchemas\\ServiceEndpoint",
            "serviceendpointschema.xml",
            "ServiceEndpoint",
            ConnectionHelper.GetOrganizationalServiceSource(),
            ConnectionHelper.GetOrganizationalServiceSource(),
            true,
            true)
        {
        }

        protected override CrmExporterConfig GetExporterConfig()
        {
            var exportConfig = base.GetExporterConfig();

            var dictionary = new Dictionary<string, List<string>>
            {
                { "serviceendpointid", new List<string> { "name" } }
            };

            exportConfig.LookupMapping.Add("serviceendpoint", dictionary);

            exportConfig.ExcludedFields.AddRange(new List<string> { "serviceendpointid", "name" });

            return exportConfig;
        }

        protected override CrmImportConfig GetImporterConfig()
        {
            var impConfig = base.GetImporterConfig();

            impConfig.MigrationConfig.SourceRootBUName = "csmhubdev4";

            return impConfig;
        }
    }
}