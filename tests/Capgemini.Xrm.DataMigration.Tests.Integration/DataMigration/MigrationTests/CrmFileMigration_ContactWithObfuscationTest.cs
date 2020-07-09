using System;
using System.Collections.Generic;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataMigration.Resiliency.Polly;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.CrmStore.DataStores;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using Capgemini.Xrm.DataMigration.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.IntegrationTests.DataMigration.MigrationTests
{
    [TestClass]
    public class CrmFileMigration_ContactWithObfuscationTest : CrmFileMigrationBaseTest
    {
        private ConsoleLogger logger = new ConsoleLogger();

        //Ci AUth in sprint auto test
        private Guid sourceUserId = Guid.Parse("fdd460bf-e73a-e711-8104-5065f38b6621");

        //Data Migration Dev in sprint auto test
        private Guid targetGuidId = Guid.Parse("04fd61b7-c33b-e711-8102-5065f38aea41");

        public CrmFileMigration_ContactWithObfuscationTest() : base(
            "ImportSchemas\\ContactSchema",
            "ContactSchemaWithOwner.xml",
            "ContactWithOwner",
            ConnectionHelper.GetOrganizationalServiceSource(),
            ConnectionHelper.GetOrganizationalServiceTarget(),
            true)
        {
        }

        [TestMethod]
        public void DirectMigrationTest()
        {
            ConsoleLogger.LogLevel = 5;

            var orgService = ConnectionHelper.GetOrganizationalServiceSource();
            var repo = new EntityRepository(orgService, new ServiceRetryExecutor());

            List<string> fetchXMLQueries = new List<string>
            {
                "<fetch><entity name=\"contact\" ><attribute name=\"ownerid\" /><attribute name=\"firstname\" /></entity></fetch>"
            };

            var readerConfig = new CrmStoreReaderConfig(fetchXMLQueries)
            {
                BatchSize = 2,
                PageSize = 2,
                TopCount = 2,
            };

            var writerConfig = new CrmStoreWriterConfig
            {
                SaveBatchSize = 200,

            };


            var reader = new DataCrmStoreReader(logger, repo, readerConfig);
            var writer = new DataCrmStoreWriter(logger, repo, writerConfig);

            var migrator = new GenericDataMigrator<Entity, EntityWrapper>(logger, reader, writer);
            var obfuscateProcessor = new ObfuscateFieldsProcessor(repo.GetEntityMetadataCache, GetImporterConfig().FieldsToObfuscate);

            migrator.AddProcessor(obfuscateProcessor);

            migrator.MigrateData();
        }

        protected override CrmExporterConfig GetExporterConfig()
        {
            var exportConfig = base.GetExporterConfig();

            exportConfig.BatchSize = 500;
            exportConfig.PageSize = 500;
            exportConfig.TopCount = 1000;

            return exportConfig;
        }

        protected override CrmImportConfig GetImporterConfig()
        {
            var importConfig = base.GetImporterConfig();

            importConfig.SaveBatchSize = 500;

            List<FieldToBeObfuscated> fiedlsToBeObfuscated = new List<FieldToBeObfuscated>();
            fiedlsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "firstname" });

            EntityToBeObfuscated entityToBeObfuscated = new EntityToBeObfuscated() { EntityName = "contact", FieldsToBeObfuscated = fiedlsToBeObfuscated };

            var fieldToBeObfuscated = new List<EntityToBeObfuscated>();
            fieldToBeObfuscated.Add(entityToBeObfuscated);

            importConfig.FieldsToObfuscate = fieldToBeObfuscated;

            return importConfig;
        }
    }
}
