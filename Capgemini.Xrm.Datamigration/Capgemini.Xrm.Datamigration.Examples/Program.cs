using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Resiliency.Polly;
using Capgemini.Xrm.Datamigration.Examples.Properties;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.Engine;
using Capgemini.Xrm.DataMigration.Repositories;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capgemini.Xrm.Datamigration.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            ExportDataXml(Settings.Default.CrmExportConnectionString);
        }

        static void ExportDataXml(string connectionString)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var serviceClient = new CrmServiceClient(connectionString);

            var exportConfig = new CrmExporterConfig()
            {
                BatchSize = 1000,
                PageSize = 500,
                FilePrefix = "EX0.1",
                JsonFolderPath = "Extract",
                OneEntityPerBatch = true,
                SeperateFilesPerEntity = true,
                TopCount = 10000
            };

            var entityRepo = new EntityRepository(serviceClient, new ServiceRetryExecutor());

            var fileExporter = new CrmFileDataExporter(new ConsoleLogger(), entityRepo, exportConfig, cancellationTokenSource.Token);

            fileExporter.MigrateData();
        }
    }
}
