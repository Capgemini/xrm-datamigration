using System.Collections.Generic;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.IntegrationTests.DataMigration.MigrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmFileMigrationBusinessUnitTests : CrmFileMigrationBaseTest
    {
        public CrmFileMigrationBusinessUnitTests()
            : base(
            "TestData\\ImportSchemas\\BusinessUnits",
            "BusinessUnitSchema.xml",
            "BusinessUnit",
            ConnectionHelper.GetOrganizationalServiceSource(),
            ConnectionHelper.GetOrganizationalServiceTarget())
        {
        }

        protected override CrmExporterConfig GetExporterConfig()
        {
            var exportConfig = base.GetExporterConfig();

            exportConfig.CrmMigrationToolSchemaFilters.Add("businessunit", "<filter> <condition attribute=\"parentbusinessunitid\" operator=\"not-null\" /> </filter>");

            return exportConfig;
        }

        protected override CrmImportConfig GetImporterConfig()
        {
            var config = base.GetImporterConfig();
            config.MigrationConfig = new MappingConfiguration
            {
                ApplyAliasMapping = true
            };

            return config;
        }
    }
}