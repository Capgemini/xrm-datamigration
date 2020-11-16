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
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.IntegrationTests.DataMigration.MigrationTests
{
    [TestClass]
    public class CrmFileMigrationContactWithObfuscationTest : CrmFileMigrationBaseTest
    {
        private readonly ConsoleLogger logger = new ConsoleLogger();

        public CrmFileMigrationContactWithObfuscationTest()
            : base(
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

            Action result = () => migrator.MigrateData();

            result.Should().NotThrow();
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

            var fiedlsToBeObfuscated = new List<FieldToBeObfuscated>
            {
                new FieldToBeObfuscated() { FieldName = "firstname" }
            };

            var entityToBeObfuscated = new EntityToBeObfuscated() { EntityName = "contact" };
            entityToBeObfuscated.FieldsToBeObfuscated.AddRange(fiedlsToBeObfuscated);
            var fieldToBeObfuscated = new List<EntityToBeObfuscated>
            {
                entityToBeObfuscated
            };

            importConfig.FieldsToObfuscate.AddRange(fieldToBeObfuscated);

            return importConfig;
        }
    }
}