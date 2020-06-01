using System;
using System.Collections.Generic;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.IntegrationTests.DataMigration.MigrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmFileMigrationTeamsTests : CrmFileMigrationBaseTest
    {
        public CrmFileMigrationTeamsTests()
            : base(
            "TestData\\ImportSchemas\\TeamSchema",
            "TeamsSchema.xml",
            "Teams",
            ConnectionHelper.GetOrganizationalServiceSource(),
            ConnectionHelper.GetOrganizationalServiceTarget())
        {
        }

        protected override CrmExporterConfig GetExporterConfig()
        {
            var exportConfig = base.GetExporterConfig();

            exportConfig.CrmMigrationToolSchemaFilters.Add("team", "<filter> <condition attribute=\"isdefault\" operator=\"ne\" value=\"1\" /> </filter>");
            exportConfig.CrmMigrationToolSchemaFilters.Add("queue", "<filter type=\"or\" > <condition attribute=\"name\" operator=\"not-begin-with\" value=\"&lt;\" /> <condition attribute=\"owningteam\" operator=\"not-null\" /> </filter>");

            exportConfig.LookupMapping.Add("role", new Dictionary<string, List<string>>());
            exportConfig.LookupMapping["role"].Add("businessunitid", new List<string> { "name" });
            exportConfig.LookupMapping["role"].Add("roleid", new List<string> { "name", "businessunitid" });

            exportConfig.LookupMapping.Add("team", new Dictionary<string, List<string>>());
            exportConfig.LookupMapping["team"].Add("businessunitid", new List<string> { "name" });
            exportConfig.LookupMapping["team"].Add("teamid", new List<string> { "name", "businessunitid" });

            exportConfig.LookupMapping.Add("teamroles", new Dictionary<string, List<string>>());
            exportConfig.LookupMapping["teamroles"].Add("roleid", new List<string> { "name", "businessunitid" });
            exportConfig.LookupMapping["teamroles"].Add("teamid", new List<string> { "name", "businessunitid" });

            return exportConfig;
        }

        protected override CrmImportConfig GetImporterConfig()
        {
            var config = base.GetImporterConfig();

            var mapConfig = new MappingConfiguration()
            {
                ApplyAliasMapping = true,
                SourceRootBUName = "csmhubdev4"
            };

            config.MigrationConfig = mapConfig;

            return config;
        }
    }
}