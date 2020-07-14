using Capgemini.DataMigration.Core.Helpers;
using Capgemini.DataMigration.Core.Model;
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

            LoadObfuscationLookups();


            ExportData(Settings.Default.CrmExportConnectionString, GetSchemaPath(), GetExportPath());

            Console.WriteLine("Importing data - press enter to import");
            Console.ReadLine();
            ImportData(Settings.Default.CrmImportConnectionString, GetSchemaPath(), GetExportPath());

            Console.WriteLine("Operations completed - press enter to exit");
            Console.ReadLine();
        }

        private static void LoadObfuscationLookups()
        {
            ObfuscationLookupHelper.LoadLookups(GetLookupFolderPath());
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
                var fileExporterCsv = new CrmFileDataExporterCsv(logger, entityRepo, GetExportConfig(), schema, tokenSource.Token);
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
                BatchSize = 10,
                PageSize = 10,
                FilePrefix = $"Demo{Settings.Default.DemoScenarioName}",
                OneEntityPerBatch = true,
                SeperateFilesPerEntity = true,
                TopCount = 10,
                JsonFolderPath = GetExportPath(),
                CrmMigrationToolSchemaPaths = new List<string>() { GetSchemaPath() }
            };

            exportConfig.FieldsToObfuscate = GenerateObfuscationObject();

            var filePath = $"{GetScenarioPath()}\\ExportConfig.json";

            if (!File.Exists(filePath))
                exportConfig.SaveConfiguration(filePath);
            else
                exportConfig = CrmExporterConfig.GetConfiguration(filePath);

            exportConfig.JsonFolderPath = GetExportPath();
            exportConfig.CrmMigrationToolSchemaPaths = new List<string>() { GetSchemaPath() };

            return exportConfig;
        }

        private static List<EntityToBeObfuscated> GenerateObfuscationObject()
        {
            List<FieldToBeObfuscated> fiedlsToBeObfuscated = new List<FieldToBeObfuscated>();

            List<ObfuscationFormatOption> arguments = new List<ObfuscationFormatOption>();
            List<ObfuscationFormatOption> arguments2 = new List<ObfuscationFormatOption>();
            List<ObfuscationFormatOption> arguments3 = new List<ObfuscationFormatOption>();
            List<ObfuscationFormatOption> arguments4 = new List<ObfuscationFormatOption>();

            Dictionary<string, string> argumentsParams = new Dictionary<string, string>();
            Dictionary<string, string> argumentsParams2 = new Dictionary<string, string>();
            Dictionary<string, string> argumentsParams3 = new Dictionary<string, string>();
            Dictionary<string, string> argumentsParams4 = new Dictionary<string, string>();
            Dictionary<string, string> argumentsParams5 = new Dictionary<string, string>();
            Dictionary<string, string> argumentsParams6 = new Dictionary<string, string>();
            Dictionary<string, string> argumentsParams7 = new Dictionary<string, string>();
            Dictionary<string, string> argumentsParams8 = new Dictionary<string, string>();
            Dictionary<string, string> argumentsParams9 = new Dictionary<string, string>();



            argumentsParams.Add("filename", "FirstnameAndSurnames.csv");
            argumentsParams.Add("columnname", "firstname");

            arguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, argumentsParams));
            fiedlsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "firstname", ObfuscationFormat = "f-{0}", ObfuscationFormatArgs = arguments });

            //argumentsParams2.Add("length", "30");
            //arguments2.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomString, argumentsParams2));
            argumentsParams2.Add("filename", "FirstnameAndSurnames.csv");
            argumentsParams2.Add("columnname", "surname");

            arguments2.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, argumentsParams2));

            fiedlsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "lastname", ObfuscationFormat = "l-{0}", ObfuscationFormatArgs = arguments2 });

            //argumentsParams3.Add("length", "10");
            argumentsParams3.Add("filename", "FirstnameAndSurnames.csv");
            argumentsParams3.Add("columnname", "firstname");
            arguments3.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, argumentsParams3));

            argumentsParams4.Add("length", "30");
            arguments3.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomString, argumentsParams4));

            argumentsParams5.Add("min", "10000");
            argumentsParams5.Add("max", "99999");
            arguments3.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomNumber, argumentsParams5));

            fiedlsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "emailaddress1", ObfuscationFormat = "{0}.{1}@{2}.com", ObfuscationFormatArgs = arguments3 });

            //argumentsParams6.Add("length", "2");
            //arguments4.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomString, argumentsParams6));

            //argumentsParams8.Add("min", "1");
            //argumentsParams8.Add("max", "99");
            //arguments4.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomNumber, argumentsParams8));

            //argumentsParams9.Add("min", "1");
            //argumentsParams9.Add("max", "9");
            //arguments4.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomNumber, argumentsParams9));

            //argumentsParams7.Add("length", "2");
            //arguments4.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomString, argumentsParams7));
            argumentsParams6.Add("filename", "Postcodes.csv");
            argumentsParams6.Add("columnname", "Postcode");

            arguments4.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, argumentsParams6));
            fiedlsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "address1_postalcode", ObfuscationFormat = "PC-{0}", ObfuscationFormatArgs = arguments4 });

            EntityToBeObfuscated entityToBeObfuscated = new EntityToBeObfuscated() { EntityName = "contact", FieldsToBeObfuscated = fiedlsToBeObfuscated };

            List<EntityToBeObfuscated> EntitiesToBeObfuscated = new List<EntityToBeObfuscated>();
            EntitiesToBeObfuscated.Add(entityToBeObfuscated);

            return EntitiesToBeObfuscated;
        }

        static string GetSchemaPath()
        {
            var schemaPath = Path.Combine(GetScenarioPath(), "Schema.xml");
            return schemaPath;
        }

        static string GetLookupFolderPath()
        {
            var lookupPath = Path.Combine(GetScenarioPath(), "LookupFiles");
            return lookupPath;
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
