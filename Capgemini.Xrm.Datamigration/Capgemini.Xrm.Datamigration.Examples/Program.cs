using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Resiliency.Polly;
using Capgemini.Xrm.Datamigration.Examples.Properties;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.Engine;
using Capgemini.Xrm.DataMigration.Repositories;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capgemini.Xrm.Datamigration.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            ExportDataXml(Settings.Default.CrmExportConnectionString, GetConfigSchemaPath(), GetExportPath());
        }

        static void ExportDataXml(string connectionString, string schemaPath, string exportFolderPath)
        {
            if (!Directory.Exists(exportFolderPath))
                Directory.CreateDirectory(exportFolderPath);
           
            var cancellationTokenSource = new CancellationTokenSource();

            var serviceClient = new CrmServiceClient(connectionString);

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

            var entityRepo = new EntityRepository(serviceClient, new ServiceRetryExecutor());

            var fileExporter = new CrmFileDataExporter(new ConsoleLogger(), entityRepo, exportConfig, cancellationTokenSource.Token);

            fileExporter.MigrateData();
        }

        static string GetConfigSchemaPath()
        {
            string folderPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            var schemaPath = Path.Combine(folderPath, "ConfigExamples", "Contacts", "ContactsSchema.xml");
            return schemaPath;
        }

        static string GetExportPath()
        {
            string folderPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            var exportFolderPath = Path.Combine(folderPath, "ConfigExamples", "Contacts", "ExportedData");
            return exportFolderPath;
        }
    }
}
