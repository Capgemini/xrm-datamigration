using Capgemini.DataMigration.Core.Helpers;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataMigration.Resiliency.Polly;
using Capgemini.Xrm.Datamigration.Examples.Properties;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.Engine;
using Capgemini.Xrm.DataMigration.Repositories;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Capgemini.Xrm.Datamigration.Examples
{
    internal class Program
    {
        private static void Main(string[] args)
        {
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
                ExportData(Settings.Default.CrmExportConnectionString, GetSchemaPath());

                Console.WriteLine("Importing data - press enter to import");
                Console.ReadLine();
                ImportData(Settings.Default.CrmImportConnectionString, GetSchemaPath(), GetExportPath());

                Console.WriteLine("Operations completed - press enter to exit");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        private static void LoadObfuscationLookups()
        {
            var lookupPath = GetLookupFolderPath();

            if (Directory.Exists(lookupPath))
                ObfuscationLookupHelper.LoadLookups(lookupPath);
        }

        private static void ExportData(string connectionString, string schemaPath)
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

        private static CrmImportConfig GetImportConfig()
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

        private static CrmExporterConfig GetExportConfig()
        {
            var exportConfig = new CrmExporterConfig()
            {
                BatchSize = 1000,
                PageSize = 500,
                FilePrefix = $"Demo{Settings.Default.DemoScenarioName}",
                OneEntityPerBatch = true,
                SeperateFilesPerEntity = true,
                TopCount = 10000,
                JsonFolderPath = GetExportPath()
            };
            exportConfig.CrmMigrationToolSchemaPaths.Add(GetSchemaPath());

            if (Settings.Default.DemoScenarioName == "Obfuscation")
            {
                exportConfig.FieldsToObfuscate.AddRange(GenerateObfuscationConfig());
            }

            var filePath = $"{GetScenarioPath()}\\ExportConfig.json";

            if (!File.Exists(filePath))
                exportConfig.SaveConfiguration(filePath);
            else
                exportConfig = CrmExporterConfig.GetConfiguration(filePath);

            exportConfig.JsonFolderPath = GetExportPath();
            exportConfig.CrmMigrationToolSchemaPaths.Add(GetSchemaPath());

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

            var firstnameFieldToBeObfuscated = new FieldToBeObfuscated() { FieldName = "firstname", ObfuscationFormat = "OB-{0}" };
            firstnameFieldToBeObfuscated.ObfuscationFormatArgs.AddRange(firstnameArguments);

            fieldsToBeObfuscated.Add(firstnameFieldToBeObfuscated);

            // Lastname
            List<ObfuscationFormatOption> lastnameArguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> lastnameArgumentsParams = new Dictionary<string, string>();

            lastnameArgumentsParams.Add("filename", "FirstnameAndSurnames.csv");
            lastnameArgumentsParams.Add("columnname", "surname");

            lastnameArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, lastnameArgumentsParams));

            var lastnameFieldToBeObfuscated = new FieldToBeObfuscated() { FieldName = "lastname", ObfuscationFormat = "OB-{0}" };
            lastnameFieldToBeObfuscated.ObfuscationFormatArgs.AddRange(lastnameArguments);

            fieldsToBeObfuscated.Add(lastnameFieldToBeObfuscated);

            // Email Address
            List<ObfuscationFormatOption> emailAddressArguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> emailAddressArgumentsParams1 = new Dictionary<string, string>();
            Dictionary<string, string> emailAddressArgumentsParams2 = new Dictionary<string, string>();

            emailAddressArgumentsParams1.Add("filename", "FirstnameAndSurnames.csv");
            emailAddressArgumentsParams1.Add("columnname", "firstname");
            emailAddressArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, emailAddressArgumentsParams1));

            emailAddressArgumentsParams2.Add("length", "10");
            emailAddressArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomString, emailAddressArgumentsParams2));

            var emailaddress1FieldToBeObfuscated = new FieldToBeObfuscated() { FieldName = "emailaddress1", ObfuscationFormat = "OB-{0}.{1}@email.com" };
            emailaddress1FieldToBeObfuscated.ObfuscationFormatArgs.AddRange(emailAddressArguments);

            fieldsToBeObfuscated.Add(emailaddress1FieldToBeObfuscated);

            // Postcode
            List<ObfuscationFormatOption> postcodeArguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> postcodeArgumentParams = new Dictionary<string, string>();

            postcodeArgumentParams.Add("filename", "ukpostcodes.csv");
            postcodeArgumentParams.Add("columnname", "postcode");

            postcodeArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, postcodeArgumentParams));
            var address1PostalcodeFieldToBeObfuscated = new FieldToBeObfuscated() { FieldName = "address1_postalcode", ObfuscationFormat = "OB-{0}" };
            address1PostalcodeFieldToBeObfuscated.ObfuscationFormatArgs.AddRange(postcodeArguments);

            fieldsToBeObfuscated.Add(address1PostalcodeFieldToBeObfuscated);

            // Address Latitude
            List<ObfuscationFormatOption> latitudeArguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> latitudeArgumentParams = new Dictionary<string, string>
            {
                { "filename", "ukpostcodes.csv" },
                { "columnname", "latitude" }
            };

            latitudeArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, latitudeArgumentParams));
            var address1latitudeFieldToBeObfuscated = new FieldToBeObfuscated() { FieldName = "address1_latitude", ObfuscationFormat = "{0}" };
            address1latitudeFieldToBeObfuscated.ObfuscationFormatArgs.AddRange(latitudeArguments);

            fieldsToBeObfuscated.Add(address1latitudeFieldToBeObfuscated);

            // Address Longitude
            List<ObfuscationFormatOption> longitudeArguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> longitudeArgumentsParams = new Dictionary<string, string>
            {
                { "filename", "ukpostcodes.csv" },
                { "columnname", "longitude" }
            };
            longitudeArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, longitudeArgumentsParams));

            var address1LongitudeFieldToBeObfuscated = new FieldToBeObfuscated() { FieldName = "address1_longitude", ObfuscationFormat = "{0}" };
            address1LongitudeFieldToBeObfuscated.ObfuscationFormatArgs.AddRange(longitudeArguments);

            fieldsToBeObfuscated.Add(address1LongitudeFieldToBeObfuscated);

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

            var address1Line1FieldToBeObfuscated = new FieldToBeObfuscated()
            {
                FieldName = "address1_line1",
                ObfuscationFormat = "OB-{0} {1} Close",
            };
            address1Line1FieldToBeObfuscated.ObfuscationFormatArgs.AddRange(address_line1Arguments);

            fieldsToBeObfuscated.Add(address1Line1FieldToBeObfuscated);

            // Address 1 using defaults
            fieldsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "address1_line2" });
            fieldsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "address1_line3" });

            // Address 1 Name
            List<ObfuscationFormatOption> address1_nameArguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> address1_NameArgumentParams = new Dictionary<string, string>();

            address1_NameArgumentParams.Add("length", "20");
            address1_nameArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomString, address1_NameArgumentParams));
            var address1NameFieldToBeObfuscated = new FieldToBeObfuscated() { FieldName = "address1_name", ObfuscationFormat = "{0}" };
            address1NameFieldToBeObfuscated.ObfuscationFormatArgs.AddRange(address1_nameArguments);

            fieldsToBeObfuscated.Add(address1NameFieldToBeObfuscated);

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

            var address1MobilephoneFieldToBeObfuscated = new FieldToBeObfuscated()
            {
                FieldName = "address1_mobilephone",
                ObfuscationFormat = "OB-0{0} {1}"
            };
            address1MobilephoneFieldToBeObfuscated.ObfuscationFormatArgs.AddRange(mobilePhoneArguments);

            fieldsToBeObfuscated.Add(address1MobilephoneFieldToBeObfuscated);

            // Address 1 Telephone1
            List<ObfuscationFormatOption> telephoneArguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> telephoneArgumentParams = new Dictionary<string, string>();

            telephoneArgumentParams.Add("min", "7000");
            telephoneArgumentParams.Add("max", "7999");
            telephoneArguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomNumber, telephoneArgumentParams));

            var address1Telephone1FieldToBeObfuscated = new FieldToBeObfuscated()
            {
                FieldName = "address1_telephone1",
                ObfuscationFormat = "OB-0{0}"
            };
            address1Telephone1FieldToBeObfuscated.ObfuscationFormatArgs.AddRange(telephoneArguments);

            fieldsToBeObfuscated.Add(address1Telephone1FieldToBeObfuscated);

            EntityToBeObfuscated entityToBeObfuscated = new EntityToBeObfuscated() { EntityName = "contact" };
            entityToBeObfuscated.FieldsToBeObfuscated.AddRange(fieldsToBeObfuscated);
            return entityToBeObfuscated;
        }

        private static string GetSchemaPath()
        {
            var schemaPath = Path.Combine(GetScenarioPath(), "Schema.xml");
            return schemaPath;
        }

        private static string GetLookupFolderPath()
        {
            var lookupPath = Path.Combine(GetScenarioPath(), "LookupFiles");
            return lookupPath;
        }

        private static string GetExportPath()
        {
            var exportFolderPath = Path.Combine(GetScenarioPath(), "ExportedData");
            return exportFolderPath;
        }

        private static string GetScenarioPath()
        {
            string folderPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            var scenarioPath = Path.Combine(folderPath, "DemoScenarios", Settings.Default.DemoScenarioName);
            return scenarioPath;
        }
    }
}