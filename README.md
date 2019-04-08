# Capgemini CRM Data Migration

## Description

This Data Migration project provides a flexible powerful engine based on the XRM SDK which allows CRM Dynamics Configuration, Reference and Seed data to be extracted, stored in version control and loaded to target instances.  This allows data to be managed in the same way as code and a release can be created that can load the required data to support the released functionality.

## Table Of Contents
1. [Description](#Description)  
1. [Installation](#Installation)
1. [Usage](#Usage)
1. [Contributing](#Contributing)
1. [Credits](#Credits)
1. [License](#License)

## Installation

Clone the [Git Repository](https://github.com/Capgemini/xrm-datamigration)

Open example project (Capgemini.Xrm.Datamigration.Examples) and edit configuration file (App.config):

```xml
  <applicationSettings>
    <Capgemini.Xrm.Datamigration.Examples.Properties.Settings>
      <setting name="CrmExportConnectionString" serializeAs="String">
        <value>Url = https://sourcerepo.dynamics.com; Username=xxxx; Password=xxxx; AuthType=Office365; RequireNewInstance=True;</value>
      </setting>
      <setting name="CrmImportConnectionString" serializeAs="String">
        <value>Url = https://targetrepo.crm4.dynamics.com; Username=xxx; Password=xxxx; AuthType=Office365; RequireNewInstance=True;</value>
      </setting>
    </Capgemini.Xrm.Datamigration.Examples.Properties.Settings>
  </applicationSettings>
```
Set up some contacts example in the source CRM Instance 

Run the console application and follow messages

In the bin folder there will be output folder and files with exported data created:

![outputFilesExample.png](./.attachments/outputFilesExample.png)

In the target CRM instance you can check if all contacts are created.

## Usage

Create a new console app and add [Capgemini.Xrm.DataMigration](https://www.nuget.org/packages/Capgemini.Xrm.DataMigration.Engine) Nuget
![nugetScreen.png](./.attachments/nugetScreen.png)

Xrm DataMigration Engine classes are available to be used in any custom scenario eg.

*Export Example*
```c#
        static void ExportData(string connectionString, string schemaPath, string exportFolderPath)
        {
            if (!Directory.Exists(exportFolderPath))
                Directory.CreateDirectory(exportFolderPath);
           
            var tokenSource = new CancellationTokenSource();
            var serviceClient = new CrmServiceClient(connectionString);
            var entityRepo = new EntityRepository(serviceClient, new ServiceRetryExecutor());
            var logger = new ConsoleLogger();
            var exportConfig = new CrmExporterConfig()
            {
                BatchSize = 1000,
                PageSize = 500,
                FilePrefix = "EX0.1",
                JsonFolderPath = exportFolderPath,
                OneEntityPerBatch = true,
                SeperateFilesPerEntity = true,
                TopCount = 10000,
                CrmMigrationToolSchemaPaths = new List<string>() {schemaPath}
            };

            // Json Export
            var fileExporterJson = new CrmFileDataExporter(logger, entityRepo, exportConfig, tokenSource.Token);
            fileExporterJson.MigrateData();

            // Csv Export
            var schema = CrmSchemaConfiguration.ReadFromFile(schemaPath);
            var fileExporterCsv = new CrmFileDataExporterCsv(logger, entityRepo, exportConfig, tokenSource.Token, schema);
            fileExporterCsv.MigrateData();
        }
```

*Import Example*
```c#
        public static void ImportData(string connectionString, string schemaPath, string exportFolderPath)
        {
            var tokenSource = new CancellationTokenSource();
            var serviceClient = new CrmServiceClient(connectionString);
            var entityRepo = new EntityRepository(serviceClient, new ServiceRetryExecutor());
            var logger = new ConsoleLogger();

            var importConfig = new CrmImportConfig()
            {
                FilePrefix = "EX0.1",
                JsonFolderPath = exportFolderPath,
                SaveBatchSize = 20
            };

            // Json Import
            var fileImporterJson = new CrmFileDataImporter(logger, entityRepo, importConfig, tokenSource.Token);
            fileImporterJson.MigrateData();

            //Csv Import
            var schema = CrmSchemaConfiguration.ReadFromFile(schemaPath);
            var fileImporterCsv = new CrmFileDataImporterCsv(logger, entityRepo, importConfig, schema, tokenSource.Token);
            fileImporterCsv.MigrateData();
        }
```


The engine supports two file formats JSON and CSV and has been used for a number of scenarios on a number of projects.  It is extremely flexible and supports the migration of simple reference data entities (e.g. Titles, Countries) to more complex scenarios around Security Roles and Teams.  See wiki for a fuller list of examples (link).

Other features of the engine are the support for many-to-many relationships, application of filters, building relations via composite keys and GUID mappings. 

The engine is controlled by three configuration files, a fuller explanation of the values can be found in the wiki.
**DataSchema.xml** - Defines details of the entities and attributes that are to be extracted.

**DataExport.json** – Holds details of the schema to use, filters to be applied and other run controls.  See wiki for a more details explanation.

**DataImport.json** - Holds details of the location and prefix of the Exported files that are to be loaded.

## Contributing

To contribute to this project, report any bugs, submit new feature requests, submit changes via pull requests or even join in the overall design of the tool.

## Credits

Special thanks to the entire Capgemini community for their support in developing this tool.

## License

The Xrm Solution Audit is released under the [MIT](LICENSE) license.
