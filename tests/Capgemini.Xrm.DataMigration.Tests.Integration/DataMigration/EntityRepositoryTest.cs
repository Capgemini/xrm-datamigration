using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Capgemini.DataMigration.Resiliency;
using Capgemini.DataMigration.Resiliency.Polly;
using Capgemini.Xrm.DataMigration.Cache;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Capgemini.Xrm.DataMigration.IntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class EntityRepositoryTest
    {
        private IRetryExecutor retryExecutor = null;
        private IOrganizationService orgService = null;
        private EntityRepository entityRepository = null;

        [TestInitialize]
        public void Setup()
        {
            orgService = ConnectionHelper.GetOrganizationalServiceTarget();
            retryExecutor = new ServiceRetryExecutor();
            entityRepository = new EntityRepository(orgService, retryExecutor);
        }

        [TestMethod]
        public void CreateUpdateEntitiesCreateTest()
        {
            var entList = EntityMockHelper.EntitiesToCreate.Select(p => new EntityWrapper(p)).ToList();

            var time = new Stopwatch();
            time.Start();

            entityRepository.CreateUpdateEntities(entList);

            time.Stop();

            Assert.IsTrue(time.ElapsedMilliseconds > 0);

            Trace.WriteLine($"Operation completed in [ms]{time.ElapsedMilliseconds}");

            foreach (var item in entList)
            {
                Trace.WriteLine($"id:{item.Id} result:{item.OperationResult}");
            }
        }

        [TestMethod]
        public void CreateUpdateEnttiesCreateConfigurationParaemters()
        {
            var entList = EntityMockHelper.EntitiesToCreateConfigurationParatmers.Select(p => new EntityWrapper(p)).ToList();

            var time = new Stopwatch();
            time.Start();

            entityRepository.CreateUpdateEntities(entList);
            time.Stop();

            Assert.IsTrue(time.ElapsedMilliseconds > 0);

            Trace.WriteLine("Operation completed in [ms]" + time.ElapsedMilliseconds);

            foreach (var item in entList)
            {
                Trace.WriteLine("id:" + item.Id + " result:" + item.OperationResult);
            }
        }

        [TestMethod]
        public void CreateUpdateEntitiesUpdateTest()
        {
            var entList = GetContacts(orgService, 50);
            foreach (var item in entList)
            {
                item.OriginalEntity["lastname"] = "Changed" + DateTime.Now.Ticks;
            }

            var time = new Stopwatch();
            time.Start();

            entityRepository.CreateUpdateEntities(entList);

            time.Stop();

            Assert.IsTrue(time.ElapsedMilliseconds > 0);

            Trace.WriteLine("Operation completed in [ms]" + time.ElapsedMilliseconds);

            foreach (var item in entList)
            {
                Trace.WriteLine("id:" + item.Id + " result:" + item.OperationResult);
            }
        }

        [TestMethod]
        public void FailedMultipleRecordsTest()
        {
            var entList = EntityMockHelper.EntitiesToCreate.Select(p => new EntityWrapper(p)).ToList();
            foreach (var item in entList)
            {
                item.OriginalEntity["failName"] = "Fail" + DateTime.Now.Ticks;
            }

            var time = new Stopwatch();
            time.Start();

            entityRepository.CreateUpdateEntities(entList);

            time.Stop();

            Assert.IsTrue(time.ElapsedMilliseconds > 0);

            Trace.WriteLine("Operation completed in [ms]" + time.ElapsedMilliseconds);

            foreach (var item in entList)
            {
                Trace.WriteLine("id:" + item.Id + " result:" + item.OperationResult);
            }
        }

        [TestMethod]
        public void IsManyToManyTest()
        {
            var cache = new EntityMetadataCache(orgService);

            var isManytoMany = cache.GetManyToManyEntityDetails("queuemembership");
            var isManyToMany2 = cache.GetManyToManyEntityDetails("queuemembership");

            Assert.IsNotNull(isManytoMany);
            Assert.IsNotNull(isManyToMany2);
        }

        [TestMethod]
        public void GetEntitiesByNameProcesses()
        {
            var results = entityRepository.GetEntitiesByName("workflow", new string[] { "name", "statuscode", "statecode" }, 5000);

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void GetEntitiesByNameProcessesAllFields()
        {
            var results = entityRepository.GetEntitiesByName("workflow", null, 5000);

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void GetEntitiesByNameSDKSteps()
        {
            var nameColumn = "name";
            var statuscodeColumn = "statuscode";
            var statecodeColumn = "statecode";
            var results = entityRepository.GetEntitiesByName("sdkmessageprocessingstep", new string[] { nameColumn, statuscodeColumn, statecodeColumn }, 5000);

            Assert.IsTrue(results.Count > 0);
            foreach (var item in results)
            {
                Assert.IsTrue(item.Attributes.Count == 4);

                Assert.IsTrue(item.Attributes.Keys.Contains(nameColumn));
                Assert.IsTrue(item.Attributes.Keys.Contains(statecodeColumn));
                Assert.IsTrue(item.Attributes.Keys.Contains(statuscodeColumn));
            }
        }

        [TestMethod]
        public void GetEntitiesByNameSDKStepsAllFields()
        {
            var results = entityRepository.GetEntitiesByName("sdkmessageprocessingstep", null, 5000);

            Assert.IsTrue(results.Count > 0);
            foreach (var item in results)
            {
                Assert.IsTrue(item.Attributes.Count > 3);
            }
        }

        [TestMethod]
        public void GetTotalRecordCountActiveContacts()
        {
            var count = entityRepository.GetTotalRecordCount("contact", new string[] { "statecode" }, new object[] { 0 });

            Assert.IsTrue(count > 0);
        }

        private List<EntityWrapper> GetContacts(IOrganizationService orgService, int count)
        {
            var query = new QueryExpression("contact")
            {
                ColumnSet = new ColumnSet("contactid", "firstname", "lastname"),
                TopCount = count,
                Criteria = new FilterExpression(LogicalOperator.And)
            };

            query.Criteria.AddCondition("firstname", ConditionOperator.Equal, "Test");

            var ents = orgService.RetrieveMultiple(query);

            var rec = ents.Entities.Select(p => new EntityWrapper(p)).ToList();
            return rec;
        }
    }
}