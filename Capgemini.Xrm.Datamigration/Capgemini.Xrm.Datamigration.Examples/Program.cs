
using Capgemini.DataMigration.Resiliency.Polly;
using Capgemini.Xrm.Datamigration.Examples;
using Capgemini.Xrm.Datamigration.Examples.Properties;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.Engine;
using Capgemini.Xrm.DataMigration.Repositories;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;

namespace Capgemini.Xrm.Datamigration.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ConsoleLogger.LogLevel = 5;
            Console.WriteLine($"Using demo scenario {Settings.Default.DemoScenarioName}");
            Console.WriteLine("Exporting data - press enter to export");
            Console.ReadLine();

            if (Directory.Exists(GetExportPath()))
            {
                Console.WriteLine($"Folder Exists {GetExportPath()} - press enter to delete and continue");
                Console.ReadLine();
                Directory.Delete(GetExportPath(), true);
            }

            Directory.CreateDirectory(GetExportPath());

            ExportData(Settings.Default.CrmExportConnectionString, GetSchemaPath(), GetExportPath());

            Console.WriteLine("Importing data - press enter to import");
            Console.ReadLine();
            ImportData(Settings.Default.CrmImportConnectionString, GetSchemaPath(), GetExportPath());

            Console.WriteLine("Operations completed - press enter to exit");
            Console.ReadLine();
        }

        static void ExportData(string connectionString, string schemaPath, string exportFolderPath)
        {
            Console.WriteLine("Export Started");

            var tokenSource = new CancellationTokenSource();
            var serviceClient = new CrmServiceClient(connectionString);
            var entityRepo = new EntityRepository(serviceClient, new ServiceRetryExecutor());
            var logger = new ConsoleLogger();

            if (!Settings.Default.UseCsvImport)
            {
                // Json Export
                var fileExporterJson = new CrmFileDataExporter(logger, entityRepo, GetExportConfig(), tokenSource.Token);
                fileExporterJson.MigrateData();
            }
            else
            {
                // Csv Export
                var schema = CrmSchemaConfiguration.ReadFromFile(schemaPath);
                var fileExporterCsv = new CrmFileDataExporterCsv(logger, entityRepo, GetExportConfig(), tokenSource.Token, schema);
                fileExporterCsv.MigrateData();
            }

            Console.WriteLine("Export Finished");
        }

        public static void ImportData(string connectionString, string schemaPath, string exportFolderPath)
        {
            Console.WriteLine("Import Started");

            var tokenSource = new CancellationTokenSource();
            var serviceClient = new CrmServiceClient(connectionString);
            var entityRepo = new EntityRepository(serviceClient, new ServiceRetryExecutor());
            var logger = new ConsoleLogger();

            if (!Settings.Default.UseCsvImport)
            {
                // Json Import
                var fileImporterJson = new CrmFileDataImporter(logger, entityRepo, GetImportConfig(), tokenSource.Token);
                fileImporterJson.MigrateData();
            }
            else
            {
                //Csv Import
                var schema = CrmSchemaConfiguration.ReadFromFile(schemaPath);
                var fileImporterCsv = new CrmFileDataImporterCsv(logger, entityRepo, GetImportConfig(), schema, tokenSource.Token);
                fileImporterCsv.MigrateData();
            }

            Console.WriteLine("Import Finished");
        }

        #region Helpers

        static CrmImportConfig GetImportConfig()
        {
            var importConfig = new CrmImportConfig()
            {
                FilePrefix = $"Demo{Settings.Default.DemoScenarioName}",
                SaveBatchSize = 50,
                JsonFolderPath = GetExportPath(),
            };

            var filePath = $"{GetScenarioPath()}\\ImportConfig.json";

            if (!File.Exists(filePath))
                importConfig.SaveConfiguration(filePath);
            else
                importConfig = CrmImportConfig.GetConfiguration(filePath);

            importConfig.JsonFolderPath = GetExportPath();

            return importConfig;
        }

        static CrmExporterConfig GetExportConfig()
        {
            var exportConfig = new CrmExporterConfig()
            {
                BatchSize = 1000,
                PageSize = 500,
                FilePrefix = $"Demo{Settings.Default.DemoScenarioName}",
                OneEntityPerBatch = true,
                SeperateFilesPerEntity = true,
                TopCount = 10000,
                CrmMigrationToolSchemaFilters = new Dictionary<string, string> { },
                JsonFolderPath = GetExportPath(),
                CrmMigrationToolSchemaPaths = new List<string>() { GetSchemaPath() }
            };

            var filePath = $"{GetScenarioPath()}\\ExportConfig.json";

            if (!File.Exists(filePath))
                exportConfig.SaveConfiguration(filePath);
            else
                exportConfig = CrmExporterConfig.GetConfiguration(filePath);

            exportConfig.JsonFolderPath = GetExportPath();
            exportConfig.CrmMigrationToolSchemaPaths = new List<string>() { GetSchemaPath() };

            return exportConfig;
        }

        static string GetSchemaPath()
        {
            var schemaPath = Path.Combine(GetScenarioPath(), "Schema.xml");
            return schemaPath;
        }

        static string GetExportPath()
        {
            var exportFolderPath = Path.Combine(GetScenarioPath(), "ExportedData");
            return exportFolderPath;
        }

        static string GetScenarioPath()
        {
            string folderPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            var scenarioPath = Path.Combine(folderPath, "DemoScenarios", Settings.Default.DemoScenarioName);
            return scenarioPath;
        }

        #endregion
    }
}
