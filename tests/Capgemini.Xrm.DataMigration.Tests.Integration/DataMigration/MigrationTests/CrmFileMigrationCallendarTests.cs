using System.Collections.Generic;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.IntegrationTests.DataMigration.MigrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmFileMigrationCallendarTests : CrmFileMigrationBaseTest
    {
        public CrmFileMigrationCallendarTests()
            : base(
            "TestData\\ImportSchemas\\CallendarSettings",
            "callendarSchema.xml",
            "Calendar",
            ConnectionHelper.GetOrganizationalServiceSource(),
            ConnectionHelper.GetOrganizationalServiceTarget())
        {
        }

        protected override CrmExporterConfig GetExporterConfig()
        {
            var exportConfig = base.GetExporterConfig();

            exportConfig.CrmMigrationToolSchemaFilters.Add("calendar", "<filter type=\"or\"> <condition attribute=\"type\" operator=\"in\"><value>1</value><value>2</value></condition> <condition attribute=\"calendarid\" operator=\"eq\" value=\"865e6aa9-7261-e711-810a-5065f38b03b1\"/> <condition attribute=\"calendarid\" operator=\"eq\" value=\"9a3259f2-526c-e711-80da-1458d04377a8\"/> <condition attribute=\"calendarid\" operator=\"eq\" value=\"8775c880-54a7-e711-80e3-3863bb35af60\"/> </filter>");

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