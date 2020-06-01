using System;
using System.Collections.Generic;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Resiliency.Polly;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.CrmStore.DataStores;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using Capgemini.Xrm.DataMigration.IntegrationTests.DataMigration.MigrationTests;
using Capgemini.Xrm.DataMigration.Repositories;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.IntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmFileMigrationContactWithOwnerTest : CrmFileMigrationBaseTest
    {
        private readonly ConsoleLogger logger = new ConsoleLogger();

        // Ci AUth in sprint auto test
        private Guid sourceUserId = Guid.Parse("fdd460bf-e73a-e711-8104-5065f38b6621");

        // Data Migration Dev in sprint auto test
        private Guid targetGuidId = Guid.Parse("04fd61b7-c33b-e711-8102-5065f38aea41");

        public CrmFileMigrationContactWithOwnerTest()
            : base(
            "TestData\\ImportSchemas\\ContactSchema",
            "ContactSchemaWithOwner.xml",
            "ContactWithOwner",
            ConnectionHelper.GetOrganizationSprintAutoTest(),
            ConnectionHelper.GetOrganizationSprintAutoTest())
        {
        }

        [TestMethod]
        public void DirectMigrationTest()
        {
            ConsoleLogger.LogLevel = 5;

            var orgService = ConnectionHelper.GetOrganizationSprintAutoTest();
            var repo = new EntityRepository(orgService, new ServiceRetryExecutor());

            List<string> fetchXMLQueries = new List<string>
            {
                "<fetch><entity name=\"contact\" ><attribute name=\"ownerid\" /><filter>" +
                $"<condition attribute=\"ownerid\" operator=\"eq\" value=\"{sourceUserId}\" />" +
                "</filter></entity></fetch>"
            };

            var readerConfig = new CrmStoreReaderConfig(fetchXMLQueries)
            {
                BatchSize = 200,
                PageSize = 200,
                TopCount = 200
            };

            var writerConfig = new CrmStoreWriterConfig
            {
                SaveBatchSize = 200
            };

            Dictionary<Guid, Guid> contactMappings =
               new Dictionary<Guid, Guid>() { { sourceUserId, targetGuidId } };

            MappingConfiguration mappingConfig = new MappingConfiguration();
            mappingConfig.Mappings.Add("systemuser", contactMappings);

            var reader = new DataCrmStoreReader(logger, repo, readerConfig);
            var writer = new DataCrmStoreWriter(logger, repo, writerConfig);

            var migrator = new GenericDataMigrator<Entity, EntityWrapper>(logger, reader, writer);
            var mappingProcessor = new MapEntityProcessor(mappingConfig, logger, repo);

            migrator.AddProcessor(mappingProcessor);

            FluentActions.Invoking(() => migrator.MigrateData())
                .Should()
                .NotThrow();
        }

        protected override CrmExporterConfig GetExporterConfig()
        {
            var exportConfig = base.GetExporterConfig();

            exportConfig.CrmMigrationToolSchemaFilters.Add(
                "contact",
                $"<filter> <condition attribute =\"ownerid\" operator=\"eq\" {$"value =\"{sourceUserId}\" /> </filter>"}");

            exportConfig.BatchSize = 500;
            exportConfig.PageSize = 500;
            exportConfig.TopCount = 1000;

            return exportConfig;
        }

        protected override CrmImportConfig GetImporterConfig()
        {
            var importConfig = base.GetImporterConfig();

            importConfig.SaveBatchSize = 500;
            importConfig.MigrationConfig = new MappingConfiguration();

            Dictionary<Guid, Guid> contactMappings =
                new Dictionary<Guid, Guid>() { { sourceUserId, targetGuidId } };

            importConfig.MigrationConfig.Mappings.Add("systemuser", contactMappings);

            return importConfig;
        }
    }
}