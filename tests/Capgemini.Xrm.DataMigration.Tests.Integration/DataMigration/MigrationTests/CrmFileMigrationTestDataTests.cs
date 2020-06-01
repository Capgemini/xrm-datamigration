using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Resiliency.Polly;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.CrmStore.DataStores;
using Capgemini.Xrm.DataMigration.Engine;
using Capgemini.Xrm.DataMigration.FileStore.DataStore;
using Capgemini.Xrm.DataMigration.Repositories;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.IntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmFileMigrationTestDataTests
    {
        [TestMethod]
        public void CrmFileDataExporterSource()
        {
            ConsoleLogger.LogLevel = 0;

            var orgService = ConnectionHelper.GetOrganizationalServiceSource();

            var repo = new EntityRepository(orgService, new ServiceRetryExecutor());

            string folderPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            string fetchXMLFolderPath = Path.Combine(folderPath, "TestData\\FetchXmlFolder");
            CrmExporterConfig config = new CrmExporterConfig()
            {
                FetchXMLFolderPath = fetchXMLFolderPath,
                BatchSize = 1000,
                PageSize = 100,
                TopCount = 10000
            };

            DataCrmStoreReader storeReader = new DataCrmStoreReader(new ConsoleLogger(), repo, config);
            DataFileStoreWriter storeWriter = new DataFileStoreWriter(new ConsoleLogger(), $"{DateTime.UtcNow.ToString("yyMMmmss", CultureInfo.InvariantCulture)}testexportSource", @"TestData");

            CrmFileDataExporter fileExporter = new CrmFileDataExporter(new ConsoleLogger(), storeReader, storeWriter);

            FluentActions.Invoking(() => fileExporter.MigrateData())
                .Should()
                .NotThrow();
        }

        [TestMethod]
        public void CrmFileDataExporterTarget()
        {
            ConsoleLogger.LogLevel = 0;

            var orgService = ConnectionHelper.GetOrganizationalServiceTarget();

            EntityRepository entityRepo = new EntityRepository(orgService, new ServiceRetryExecutor());

            string folderPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            string fetchXMLFolderPath = Path.Combine(folderPath, "TestData\\FetchXmlFolder");
            CrmExporterConfig config = new CrmExporterConfig()
            {
                FetchXMLFolderPath = fetchXMLFolderPath,
                BatchSize = 1000,
                PageSize = 100,
                TopCount = 10000
            };

            DataCrmStoreReader storeReader = new DataCrmStoreReader(new ConsoleLogger(), entityRepo, config);
            DataFileStoreWriter storeWriter = new DataFileStoreWriter(new ConsoleLogger(), $"{DateTime.UtcNow.ToString("yyMMmmss", CultureInfo.InvariantCulture)}testexportTarget", @"TestData");

            CrmFileDataExporter fileExporter = new CrmFileDataExporter(new ConsoleLogger(), storeReader, storeWriter);

            FluentActions.Invoking(() => fileExporter.MigrateData())
                .Should()
                .NotThrow();
        }

        [TestMethod]
        public void CrmFileDataImporterWithIgnoredFieldsTest()
        {
            ConsoleLogger.LogLevel = 3;

            using (CancellationTokenSource tokenSource = new CancellationTokenSource())
            {
                List<IEntityRepository> entRep = new List<IEntityRepository>
                {
                    new EntityRepository(ConnectionHelper.GetOrganizationalServiceTarget(), new ServiceRetryExecutor()),
                    new EntityRepository(ConnectionHelper.GetOrganizationalServiceTarget(), new ServiceRetryExecutor()),
                    new EntityRepository(ConnectionHelper.GetOrganizationalServiceTarget(), new ServiceRetryExecutor()),
                    new EntityRepository(ConnectionHelper.GetOrganizationalServiceTarget(), new ServiceRetryExecutor())
                };

                CrmImportConfig importConfig = CrmImportConfig.GetConfiguration(@"TestData\ImportConfig.json");

                CrmFileDataImporter fileExporter = new CrmFileDataImporter(new ConsoleLogger(), entRep, importConfig, tokenSource.Token);

                fileExporter.MigrateData();

                Assert.IsNotNull(fileExporter);
            }
        }
    }
}