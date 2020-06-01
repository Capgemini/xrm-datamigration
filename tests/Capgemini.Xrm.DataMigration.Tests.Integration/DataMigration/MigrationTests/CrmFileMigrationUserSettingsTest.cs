using System.Collections.Generic;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.IntegrationTests.DataMigration.MigrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.IntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmFileMigrationUserSettingsTest : CrmFileMigrationBaseTest
    {
        public CrmFileMigrationUserSettingsTest()
            : base(
            "TestData\\ImportSchemas\\UserSettings",
            "usersettingsschema.xml",
            "UserSettings",
            ConnectionHelper.GetOrganizationalServiceSource(),
            ConnectionHelper.GetOrganizationalServiceTarget(),
            true,
            true)
        {
        }

        protected override CrmExporterConfig GetExporterConfig()
        {
            var exportConfig = base.GetExporterConfig();

            exportConfig.LookupMapping.Add("systemuser", new Dictionary<string, List<string>>());
            exportConfig.LookupMapping["systemuser"].Add("systemuserid", new List<string> { "domainname" });
            exportConfig.LookupMapping["systemuser"].Add("businessunitid", new List<string> { "name" });

            exportConfig.LookupMapping.Add("systemuserroles", new Dictionary<string, List<string>>());
            exportConfig.LookupMapping["systemuserroles"].Add("systemuserid", new List<string> { "domainname", "accessmode" });
            exportConfig.LookupMapping["systemuserroles"].Add("roleid", new List<string> { "name", "businessunitid" });

            exportConfig.LookupMapping.Add("teammembership", new Dictionary<string, List<string>>());
            exportConfig.LookupMapping["teammembership"].Add("systemuserid", new List<string> { "domainname" });
            exportConfig.LookupMapping["teammembership"].Add("teamid", new List<string> { "name", "businessunitid" });

            exportConfig.LookupMapping.Add("systemuserprofiles", new Dictionary<string, List<string>>());
            exportConfig.LookupMapping["systemuserprofiles"].Add("systemuserid", new List<string> { "domainname" });
            exportConfig.LookupMapping["systemuserprofiles"].Add("fieldsecurityprofileid", new List<string> { "name" });

            exportConfig.LookupMapping.Add("usersettings", new Dictionary<string, List<string>>());
            exportConfig.LookupMapping["usersettings"].Add("systemuserid", new List<string> { "domainname" });

            exportConfig.ExcludedFields.AddRange(new List<string> { "systemuserid", "roleid", "teamid", "fieldsecurityprofileid", "systemuserprofilesid", "systemuserrolesid", "teammembershipid", "businessunitid" });

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