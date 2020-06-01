using System.Collections.Generic;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.IntegrationTests.DataMigration.MigrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmFileMigrationKBArticlesTests : CrmFileMigrationBaseTest
    {
        public CrmFileMigrationKBArticlesTests()
            : base(
            "TestData\\ImportSchemas\\KBSchema",
            "KBSchema.xml",
            "KBSchema",
            ConnectionHelper.GetOrganizationalServiceSource(),
            ConnectionHelper.GetOrganizationalServiceTarget())
        {
        }

        protected override CrmExporterConfig GetExporterConfig()
        {
            var exportConfig = base.GetExporterConfig();

            exportConfig.BatchSize = 10000;
            exportConfig.PageSize = 2500;
            exportConfig.TopCount = 20000;
            exportConfig.CrmMigrationToolSchemaFilters.Add("knowledgearticle", "<filter><condition attribute=\"isrootarticle\" operator=\"eq\" value=\"0\" /></filter>");

            return exportConfig;
        }

        protected override CrmImportConfig GetImporterConfig()
        {
            var importConfig = base.GetImporterConfig();

            importConfig.IgnoreStatuses = true;
            importConfig.IgnoreStatusesExceptions.AddRange(new List<string>() { "knowledgearticle" });
            importConfig.MigrationConfig = new MappingConfiguration
            {
                ApplyAliasMapping = true
            };

            return importConfig;
        }
    }
}