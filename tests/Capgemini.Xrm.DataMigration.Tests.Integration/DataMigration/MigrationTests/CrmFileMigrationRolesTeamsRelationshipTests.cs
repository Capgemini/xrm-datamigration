using System.Collections.Generic;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.IntegrationTests.DataMigration.MigrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmFileMigrationRolesTeamsRelationshipTests : CrmFileMigrationBaseTest
    {
        public CrmFileMigrationRolesTeamsRelationshipTests()
            : base(
            "TestData\\ImportSchemas\\RolesTeamsRelationship",
            "teamsrolesschema.xml",
            "TeamRoles",
            ConnectionHelper.GetOrganizationalServiceSource(),
            ConnectionHelper.GetOrganizationalServiceTarget(),
            true)
        {
        }

        protected override CrmExporterConfig GetExporterConfig()
        {
            var exportConfig = base.GetExporterConfig();

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
            config.MigrationConfig = new MappingConfiguration
            {
                ApplyAliasMapping = true,
                SourceRootBUName = "csmhubdev4"
            };
            return config;
        }
    }
}