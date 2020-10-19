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

            ConsoleLogger.LogLevel = 3;
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

            try
            {
                ExportData(Settings.Default.CrmExportConnectionString, GetSchemaPath(), GetExportPath());

                Console.WriteLine("Importing data - press enter to import");
                Console.ReadLine();
                ImportData(Settings.Default.CrmImportConnectionString, GetSchemaPath(), GetExportPath());

                Console.WriteLine("Operations completed - press enter to exit");
                Console.ReadLine();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

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
                BatchSize = 1000,
                PageSize = 500,
                FilePrefix = $"Demo{Settings.Default.DemoScenarioName}",
                OneEntityPerBatch = true,
                SeperateFilesPerEntity = true,
                TopCount = 10000,
                JsonFolderPath = GetExportPath(),
                CrmMigrationToolSchemaPaths = new List<string>() { GetSchemaPath() }
            };

            if (Settings.Default.DemoScenarioName == "Obfuscation")
                exportConfig.FieldsToObfuscate = GenerateObfuscationConfig();

            var filePath = $"{GetScenarioPath()}\\ExportConfig.json";

            if (!File.Exists(filePath))
                exportConfig.SaveConfiguration(filePath);
            else
                exportConfig = CrmExporterConfig.GetConfiguration(filePath);

            exportConfig.JsonFolderPath = GetExportPath();
            exportConfig.CrmMigrationToolSchemaPaths = new List<string>() { GetSchemaPath() };

            return exportConfig;
        }

        private static List<EntityToBeObfuscated> GenerateObfuscationConfig()
        {
            List<EntityToBeObfuscated> EntitiesToBeObfuscated = new List<EntityToBeObfuscated>();
            EntitiesToBeObfuscated.Add(CreateObfuscationConfigForContact());

            return EntitiesToBeObfuscated;
        }

        private static EntityToBeObfuscated CreateObfuscationConfigForContact()
        {
            List<FieldToBeObfuscated> fieldsToBeObfuscated = new List<FieldToBeObfuscated>();

            // Firstname
            List<ObfuscationFormatOption> firstnameArguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> firstnameArgumentParams = new Dictionary<string, string>();

            firstnameArgumentParams.Add("filename", "FirstnameAndSurnames.csv");
            firstnameArgumentParams.Add("columnname", "firstname");

            firstnameArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, firstnameArgumentParams));
            fieldsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "firstname", ObfuscationFormat = "OB-{0}", ObfuscationFormatArgs = firstnameArguments });

            // Lastname
            List<ObfuscationFormatOption> lastnameArguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> lastnameArgumentsParams = new Dictionary<string, string>();

            lastnameArgumentsParams.Add("filename", "FirstnameAndSurnames.csv");
            lastnameArgumentsParams.Add("columnname", "surname");

            lastnameArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, lastnameArgumentsParams));

            fieldsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "lastname", ObfuscationFormat = "OB-{0}", ObfuscationFormatArgs = lastnameArguments });

            // Email Address
            List<ObfuscationFormatOption> emailAddressArguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> emailAddressArgumentsParams1 = new Dictionary<string, string>();
            Dictionary<string, string> emailAddressArgumentsParams2 = new Dictionary<string, string>();

            emailAddressArgumentsParams1.Add("filename", "FirstnameAndSurnames.csv");
            emailAddressArgumentsParams1.Add("columnname", "firstname");
            emailAddressArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, emailAddressArgumentsParams1));

            emailAddressArgumentsParams2.Add("length", "10");
            emailAddressArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomString, emailAddressArgumentsParams2));

            fieldsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "emailaddress1", ObfuscationFormat = "OB-{0}.{1}@email.com", ObfuscationFormatArgs = emailAddressArguments });

            // Postcode
            List<ObfuscationFormatOption> postcodeArguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> postcodeArgumentParams = new Dictionary<string, string>();

            postcodeArgumentParams.Add("filename", "ukpostcodes.csv");
            postcodeArgumentParams.Add("columnname", "postcode");

            postcodeArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, postcodeArgumentParams));
            fieldsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "address1_postalcode", ObfuscationFormat = "OB-{0}", ObfuscationFormatArgs = postcodeArguments });

            // Address Latitude
            List<ObfuscationFormatOption> latitudeArguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> latitudeArgumentParams = new Dictionary<string, string>();

            latitudeArgumentParams.Add("filename", "ukpostcodes.csv");
            latitudeArgumentParams.Add("columnname", "latitude");

            latitudeArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, latitudeArgumentParams));
            fieldsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "address1_latitude", ObfuscationFormat = "{0}", ObfuscationFormatArgs = latitudeArguments });

            // Address Longitude
            List<ObfuscationFormatOption> longitudeArguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> longitudeArgumentsParams = new Dictionary<string, string>();

            longitudeArgumentsParams.Add("filename", "ukpostcodes.csv");
            longitudeArgumentsParams.Add("columnname", "longitude");
            longitudeArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, longitudeArgumentsParams));
            fieldsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "address1_longitude", ObfuscationFormat = "{0}", ObfuscationFormatArgs = longitudeArguments });

            // Address 1 using default
            fieldsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "address1_city" });
            fieldsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "address1_country" });
            fieldsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "address1_county" });

            //Address line 1
            List<ObfuscationFormatOption> address_line1Arguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> address_line1ArgumentParams = new Dictionary<string, string>();
            address_line1ArgumentParams.Add("min", "1");
            address_line1ArgumentParams.Add("max", "1234");
            address_line1Arguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomNumber, address_line1ArgumentParams));

            Dictionary<string, string> argumentsParams11_2 = new Dictionary<string, string>();
            argumentsParams11_2.Add("length", "8");
            address_line1Arguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomString, argumentsParams11_2));

            fieldsToBeObfuscated.Add(new FieldToBeObfuscated()
            {
                FieldName = "address1_line1",
                ObfuscationFormat = "OB-{0} {1} Close",
                ObfuscationFormatArgs = address_line1Arguments
            });

            // Address 1 using defaults
            fieldsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "address1_line2" });
            fieldsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "address1_line3" });

            // Address 1 Name
            List<ObfuscationFormatOption> address1_nameArguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> address1_NameArgumentParams = new Dictionary<string, string>();

            address1_NameArgumentParams.Add("length", "20");
            address1_nameArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomString, address1_NameArgumentParams));
            fieldsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "address1_name", ObfuscationFormat="{0}", ObfuscationFormatArgs = address1_nameArguments });


            // Mobile Telephone
            List<ObfuscationFormatOption> mobilePhoneArguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> mobilephoneArgumentParams1 = new Dictionary<string, string>();
            Dictionary<string, string> mobilephoneArgumentParams2 = new Dictionary<string, string>();

            mobilephoneArgumentParams1.Add("min", "7000");
            mobilephoneArgumentParams1.Add("max", "7999");
            mobilePhoneArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomNumber, mobilephoneArgumentParams1));

            mobilephoneArgumentParams2.Add("min", "123456");
            mobilephoneArgumentParams2.Add("max", "987654");
            mobilePhoneArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomNumber, mobilephoneArgumentParams2));

            fieldsToBeObfuscated.Add(new FieldToBeObfuscated()
            {
                FieldName = "address1_mobilephone",
                ObfuscationFormat = "OB-0{0} {1}",
                ObfuscationFormatArgs = mobilePhoneArguments
            });

            // Address 1 Telephone1
            List<ObfuscationFormatOption> telephoneArguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> telephoneArgumentParams = new Dictionary<string, string>();

            telephoneArgumentParams.Add("min", "7000");
            telephoneArgumentParams.Add("max", "7999");
            telephoneArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomNumber, telephoneArgumentParams));

            fieldsToBeObfuscated.Add(new FieldToBeObfuscated()
            {
                FieldName = "address1_telephone1",
                ObfuscationFormat = "OB-0{0}",
                ObfuscationFormatArgs = telephoneArguments
            });

            EntityToBeObfuscated entityToBeObfuscated = new EntityToBeObfuscated() { EntityName = "contact", FieldsToBeObfuscated = fieldsToBeObfuscated };
            return entityToBeObfuscated;
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
