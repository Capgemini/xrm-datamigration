using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Resiliency.Polly;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.Engine;
using Capgemini.Xrm.DataMigration.Repositories;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.IntegrationTests.DataMigration.MigrationTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public abstract class CrmFileMigrationBaseTest
    {
        private readonly List<IOrganizationService> targetServices;
        private readonly int threadsNo = 1;
        private readonly bool useCSV;
        private readonly bool useFakeRepo;

        protected CrmFileMigrationBaseTest(string schemaFolderPath, string schemaFileName, string configPrefix, IOrganizationService sourceService, List<IOrganizationService> targetServices, bool useCSV = false, bool useFakeRepo = false, int inputThreadsNo = 1)
            : this(schemaFolderPath, schemaFileName, configPrefix, sourceService, targetServices.FirstOrDefault(), useCSV, useFakeRepo)
        {
            threadsNo = inputThreadsNo;
            this.targetServices = targetServices;
        }

        protected CrmFileMigrationBaseTest(string schemaFolderPath, string schemaFileName, string configPrefix, IOrganizationService sourceService, IOrganizationService targetService, bool useCsv = false, bool useFakeRepo = false)
        {
            targetServices = new List<IOrganizationService> { targetService };

            SchemaFolderPath = schemaFolderPath;
            SchemaFileName = schemaFileName;
            ConfigPrefix = configPrefix;
            SourceService = sourceService;
            TargetService = targetService;
            useCSV = useCsv;
            this.useFakeRepo = useFakeRepo;

            string folderPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;

            SchemaFolderPathFull = Path.Combine(folderPath, SchemaFolderPath);
            SchemaPathFull = Path.Combine(SchemaFolderPathFull, SchemaFileName);
            ExtractedDataPathFull = Path.Combine(SchemaFolderPathFull, "ExtractedData");
        }

        public TestContext TestContext { get; set; }

        protected string ConfigPrefix { get; }

        protected string ExtractedDataPathFull { get; }

        protected string SchemaFileName { get; }

        protected string SchemaFolderPath { get; }

        protected string SchemaFolderPathFull { get; private set; }

        protected string SchemaPathFull { get; }

        protected IOrganizationService SourceService { get; }

        protected IOrganizationService TargetService { get; }

        [TestMethod]
        public void DataExportTest()
        {
            ConsoleLogger.LogLevel = 5;

            var orgService = SourceService;

            var repo = new EntityRepository(orgService, new ServiceRetryExecutor());

            var config = GetExporterConfig();

            string exportConfigPath = Path.Combine(SchemaFolderPathFull, $"{ConfigPrefix}ExportConfig.json");

            if (File.Exists(exportConfigPath))
            {
                File.Delete(exportConfigPath);
            }

            config.SaveConfiguration(exportConfigPath);

            if (Directory.Exists(ExtractedDataPathFull))
            {
                Directory.Delete(ExtractedDataPathFull, true);
            }

            Directory.CreateDirectory(ExtractedDataPathFull);

            CrmFileDataExporter fileExporter = new CrmFileDataExporter(new ConsoleLogger(), repo, config, new CancellationToken(false));

            FluentActions.Invoking(() => fileExporter.MigrateData())
                .Should()
                .NotThrow();

            if (useCSV)
            {
                var schema = CrmSchemaConfiguration.ReadFromFile(SchemaPathFull);
                CrmFileDataExporterCsv fileExporterCsv = new CrmFileDataExporterCsv(new ConsoleLogger(), repo, config, schema, new CancellationToken(false));

                FluentActions.Invoking(() => fileExporterCsv.MigrateData())
                .Should()
                .NotThrow();
            }
        }

        [TestMethod]
        public void DataImportCsvTest()
        {
            var message = DataImport(true);

            Assert.IsFalse(string.IsNullOrEmpty(message));
        }

        [TestMethod]
        public void DataImportJsonTest()
        {
            var message = DataImport(false);

            Assert.IsFalse(string.IsNullOrEmpty(message));
        }

        protected virtual CrmExporterConfig GetExporterConfig()
        {
            string schemaPath = SchemaPathFull;
            string extractedDataPath = ExtractedDataPathFull;

            CrmExporterConfig config = new CrmExporterConfig()
            {
                BatchSize = 500,
                PageSize = 500,
                TopCount = 100000,
                JsonFolderPath = extractedDataPath,
                OnlyActiveRecords = false,
                OneEntityPerBatch = true
            };

            config.CrmMigrationToolSchemaPaths.Add(schemaPath);

            return config;
        }

        protected virtual CrmImportConfig GetImporterConfig()
        {
            CrmImportConfig importConfig = new CrmImportConfig()
            {
                IgnoreStatuses = true,
                IgnoreSystemFields = true,
                JsonFolderPath = ExtractedDataPathFull,
                SaveBatchSize = 500,
                MigrationConfig = new MappingConfiguration()
            };

            return importConfig;
        }

        private string DataImport(bool useCsv)
        {
            ConsoleLogger.LogLevel = 5;

            var logger = new ConsoleLogger();

            List<IEntityRepository> entityRepos = new List<IEntityRepository>();
            int cnt = 0;

            while (entityRepos.Count < threadsNo)
            {
                if (useFakeRepo)
                {
                    entityRepos.Add(new EntityRepositoryFake(targetServices[cnt], new ServiceRetryExecutor(), RepositoryCachingMode.None));
                }
                else
                {
                    entityRepos.Add(new EntityRepository(targetServices[cnt], new ServiceRetryExecutor()));
                }

                cnt++;
            }

            var importConfig = GetImporterConfig();

            string importConfigPath = Path.Combine(SchemaFolderPathFull, ConfigPrefix + "ImportConfig.json");

            if (File.Exists(importConfigPath))
            {
                File.Delete(importConfigPath);
            }

            importConfig.SaveConfiguration(importConfigPath);

            CrmGenericImporter fileImporter;
            if (useCsv)
            {
                var schema = CrmSchemaConfiguration.ReadFromFile(SchemaPathFull);
                fileImporter = new CrmFileDataImporterCsv(logger, entityRepos, importConfig, schema, new CancellationToken(false));
            }
            else
            {
                fileImporter = new CrmFileDataImporter(logger, entityRepos, importConfig, new CancellationToken(false));
            }

            var watch = Stopwatch.StartNew();
            fileImporter.MigrateData();
            watch.Stop();
            var message = $"Importing Data completed, took {watch.Elapsed.TotalSeconds} seconds";
            Debug.WriteLine(message);
            return message;
        }
    }
}