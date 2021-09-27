using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.DataMigration.Exceptions;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.FileStore.DataStore;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Moq;

namespace Capgemini.Xrm.DataMigration.FileStore.UnitTests.DataStore
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DataFileStoreReaderCsvTests : UnitTestBase
    {
        private static string extractedPath = Path.Combine(TestBase.GetWorkiongFolderPath(), "TestData");
        private static string extractFolder = Path.Combine(extractedPath, "ExtractedData");
        private readonly string filePrefix = "filePrefix";
        private readonly string filesPath = "TestData";

        private CrmSchemaConfiguration crmSchemaConfiguration;

        private DataFileStoreReaderCsv systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
            crmSchemaConfiguration = new CrmSchemaConfiguration();
        }

        [TestMethod]
        [TestCategory(TestBase.AutomatedTestCategory)]
        public void ReadTestFileSystemuser()
        {
            var result = GetFirstEntities("ExportedDataSystemUser", GetSchema(extractedPath, "usersettingsschema.xml"), MockLogger.Object);
            Assert.AreEqual(result.Item1.Count, result.Item2.Count);
            List<Entity> firstEntList = result.Item1;
            List<Entity> firstEntJsonList = result.Item2;

            int idx = 0;
            while (idx < firstEntList.Count)
            {
                var firstEnt = firstEntList[idx];
                var firstEntJson = firstEntJsonList[idx];
                CompareAttributes(firstEnt, firstEntJson, true);
                idx++;
            }
        }

        [TestMethod]
        [TestCategory(TestBase.AutomatedTestCategory)]
        public void ReadTestFileUserprofile()
        {
            var result = GetFirstEntities("ExportedDataUserProfile", GetSchema(extractedPath, "usersettingsschema.xml"), MockLogger.Object);
            Assert.AreEqual(result.Item1.Count, result.Item2.Count);

            List<Entity> firstEntList = result.Item1;
            List<Entity> firstEntJsonList = result.Item2;

            int idx = 0;
            while (idx < firstEntList.Count)
            {
                var firstEnt = firstEntList[idx];
                var firstEntJson = firstEntJsonList[idx];
                CompareAttributes(firstEnt, firstEntJson);
                idx++;
            }
        }

        [TestMethod]
        [TestCategory(TestBase.AutomatedTestCategory)]
        public void ReadTestFileUserrole()
        {
            var result = GetFirstEntities("ExportedDataUserRole", GetSchema(extractedPath, "usersettingsschema.xml"), MockLogger.Object);
            Assert.AreEqual(result.Item1.Count, result.Item2.Count);

            List<Entity> firstEntList = result.Item1;
            List<Entity> firstEntJsonList = result.Item2;

            int idx = 0;
            while (idx < firstEntList.Count)
            {
                var firstEnt = firstEntList[idx];
                var firstEntJson = firstEntJsonList[idx];
                CompareAttributes(firstEnt, firstEntJson);
                idx++;
            }
        }

        [TestMethod]
        [TestCategory(TestBase.AutomatedTestCategory)]
        public void ReadTestFileTeammem()
        {
            var result = GetFirstEntities("ExportedDataTeamMem", GetSchema(extractedPath, "usersettingsschema.xml"), MockLogger.Object);
            Assert.AreEqual(result.Item1.Count, result.Item2.Count);
            List<Entity> firstEntList = result.Item1;
            List<Entity> firstEntJsonList = result.Item2;

            int idx = 0;
            while (idx < firstEntList.Count)
            {
                var firstEnt = firstEntList[idx];
                var firstEntJson = firstEntJsonList[idx];
                CompareAttributes(firstEnt, firstEntJson);
                idx++;
            }
        }

        [TestMethod]
        [TestCategory(TestBase.AutomatedTestCategory)]
        public void ReadTestFileUserset()
        {
            var result = GetFirstEntities("ExportedDataUserSet", GetSchema(extractedPath, "usersettingsschema.xml"), MockLogger.Object);
            Assert.AreEqual(result.Item1.Count, result.Item2.Count);
            List<Entity> firstEntList = result.Item1;
            List<Entity> firstEntJsonList = result.Item2;

            int idx = 0;
            while (idx < firstEntList.Count)
            {
                var firstEnt = firstEntList[idx];
                var firstEntJson = firstEntJsonList[idx];
                CompareAttributes(firstEnt, firstEntJson);
                idx++;
            }
        }

        [TestMethod]
        [TestCategory(TestBase.AutomatedTestCategory)]
        public void ReadContacts()
        {
            var expectedIdList = new List<Guid>{
                Guid.Parse("274589c3-2dbf-465b-938a-4992a24fea5b"),
                Guid.Parse("e6f72b28-c9c8-4315-b005-7fbe74495e3b"),
                Guid.Parse("4ac0c3be-a142-402f-988b-0c01c158c065"),
                Guid.Parse("07843cd6-ce3f-4638-a36f-5d7dbe4a5c26") };

            var store = new DataFileStoreReaderCsv(MockLogger.Object, "ValidFilePrefix", extractFolder, GetSchema());

            var batch = store.ReadBatchDataFromStore();
            var actual = batch.Select(p => p.OriginalEntity).ToList();

            actual.Count.Should().Be(4);

            for (int counter = 0; counter < expectedIdList.Count; counter++)
            {
                var itemLabel = counter + 1;
                var item = actual.First(x => x.GetAttributeValue<Guid>("contactid") == expectedIdList[counter]);

                item.GetAttributeValue<string>("firstname").Should().StartWith($"First Test {itemLabel}");
                item.GetAttributeValue<string>("lastname").Should().StartWith($"Last Test {itemLabel}");

                switch (itemLabel)
                {
                    case 1:
                        item.GetAttributeValue<EntityReference>("ownerid").LogicalName.Should().Be("systemuser");
                        item.GetAttributeValue<EntityReference>("ownerid").Id.ToString().Should().Be("5633ab67-20f8-4f2b-8237-f81814910707");
                        break;

                    case 2:
                        item.GetAttributeValue<EntityReference>("ownerid").LogicalName.Should().Be("systemuser");
                        item.GetAttributeValue<EntityReference>("ownerid").Id.ToString().Should().Be("fb73d6ab-86b1-4d59-a99b-b65792a0dc75");
                        break;

                    case 3:
                        item.GetAttributeValue<EntityReference>("ownerid").LogicalName.Should().Be("team");
                        item.GetAttributeValue<EntityReference>("ownerid").Id.ToString().Should().Be("91f9d682-e7aa-4506-bd8c-5f4c7f515044");
                        break;

                    case 4:
                        item.GetAttributeValue<EntityReference>("ownerid").LogicalName.Should().Be("user");
                        item.GetAttributeValue<EntityReference>("ownerid").Id.ToString().Should().Be("925f0d29-8099-4247-a0c9-149dc9cf4c6b");
                        break;
                }
            }
        }

        [TestMethod]
        [TestCategory(TestBase.AutomatedTestCategory)]
        public void ReadContactsFailsForInvalidFile()
        {
            var message = "CSV file does not contain column ownerid.LogicalName! OwnerId will be mapped to systemuser. If you wanted granular mapping of OwnerId, please regenerate the CSV.";

            var store = new DataFileStoreReaderCsv(MockLogger.Object, "ErrorFilePrefix", extractFolder, GetSchema());
            MockLogger.Setup(x => x.LogWarning(message));

            FluentActions.Invoking(() => store.ReadBatchDataFromStore())
                         .Should()
                         .NotThrow();

            MockLogger.Verify(x => x.LogWarning(message));
        }

        [TestMethod]
        [TestCategory(TestBase.AutomatedTestCategory)]
        public void Reset()
        {
            systemUnderTest = new DataFileStoreReaderCsv(MockLogger.Object, filePrefix, filesPath, crmSchemaConfiguration);

            FluentActions.Invoking(() => systemUnderTest.Reset())
                         .Should()
                         .NotThrow();
        }

        private static Tuple<List<Entity>, List<Entity>> GetFirstEntities(string filePrefix, CrmSchemaConfiguration schemaConfig, ILogger logger)
        {
            var store = new DataFileStoreReaderCsv(logger, filePrefix, extractFolder, schemaConfig);

            var batch = store.ReadBatchDataFromStore();
            List<Entity> firstEnt = batch.Select(p => p.OriginalEntity).ToList();

            var storeJson = new DataFileStoreReader(logger, filePrefix, extractFolder);

            var batchJson = storeJson.ReadBatchDataFromStore();
            List<Entity> firstEntJson = batchJson.Select(p => p.OriginalEntity).ToList();

            return new Tuple<List<Entity>, List<Entity>>(firstEnt, firstEntJson);
        }

        private static void AssertMapAttribute(Entity firstEnt, Entity firstEntJson, string attrName)
        {
            Assert.IsNotNull(firstEnt.Attributes[attrName]);
            Assert.IsNotNull(firstEntJson.Attributes[attrName]);

            var attrMap = firstEnt.GetAttributeValue<AliasedValue>(attrName);
            var attrMap2 = firstEntJson.GetAttributeValue<AliasedValue>(attrName);

            Assert.AreEqual(attrMap.AttributeLogicalName, attrMap2.AttributeLogicalName);
            Assert.AreEqual(attrMap.EntityLogicalName, attrMap2.EntityLogicalName);

            if (attrMap.Value is string)
            {
                Assert.AreEqual(attrMap.Value, attrMap2.Value);
            }
            else
            {
                var attrMapEnt = (EntityReference)attrMap.Value;
                var attrMapEnt2 = (EntityReference)attrMap2.Value;

                Assert.AreEqual(attrMapEnt.LogicalName, attrMapEnt2.LogicalName);
                Assert.AreEqual(attrMapEnt.Name, attrMapEnt2.Name);
            }
        }

        private static void CompareAttributes(Entity firstEnt, Entity firstEntJson, bool ignoreCount = false)
        {
            Assert.IsNotNull(firstEnt);
            Assert.IsNotNull(firstEntJson);
            if (!ignoreCount)
            {
                Assert.AreEqual(firstEnt.Attributes.Count, firstEntJson.Attributes.Count);
            }

            int idx = 0;

            while (idx < firstEnt.Attributes.Count)
            {
                var attr1 = firstEnt.Attributes.ToList()[idx];

                if (attr1.Value is string)
                {
                    Assert.AreEqual(firstEnt.GetAttributeValue<string>(attr1.Key), firstEntJson.GetAttributeValue<string>(attr1.Key));
                }
                else if (attr1.Value is AliasedValue)
                {
                    AssertMapAttribute(firstEnt, firstEntJson, attr1.Key);
                }

                idx++;
            }
        }
    }
}