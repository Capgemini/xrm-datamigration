using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Resiliency;
using Capgemini.DataMigration.Resiliency.Polly;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.CrmStore.DataStores;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Repositories;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.IntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class DataCrmStoreReaderWriterTest
    {
        private IRetryExecutor retryExecutor;

        [TestInitialize]
        public void Setup()
        {
            retryExecutor = new ServiceRetryExecutor();
        }

        [TestMethod]
        public void ReadBatchDataFromStore()
        {
            var orgService = ConnectionHelper.GetOrganizationalServiceSource();

            var entRepo = new EntityRepository(orgService, retryExecutor);
            string folderPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            string fetchXMLFolderPath = Path.Combine(folderPath, "TestData\\FetchXmlFolder");
            CrmExporterConfig config = new CrmExporterConfig() { FetchXMLFolderPath = fetchXMLFolderPath };

            DataCrmStoreReader crmStore = new DataCrmStoreReader(new ConsoleLogger(), entRepo, config);

            List<EntityWrapper> results = new List<EntityWrapper>();

            List<EntityWrapper> batch = crmStore.ReadBatchDataFromStore();
            while (batch.Count > 0)
            {
                results.AddRange(batch);
                batch = crmStore.ReadBatchDataFromStore();
            }

            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void SaveBatchDataToStore()
        {
            var orgService = ConnectionHelper.GetOrganizationalServiceTarget();
            var entRepo = new EntityRepository(orgService, retryExecutor);

            DataCrmStoreWriter crmStore = new DataCrmStoreWriter(new ConsoleLogger(), entRepo);

            List<EntityWrapper> toCreate = EntityMockHelper.EntitiesToCreate.Select(p => new EntityWrapper(p)).ToList();

            FluentActions.Invoking(() => crmStore.SaveBatchDataToStore(toCreate))
                .Should()
                .NotThrow();
        }
    }
}