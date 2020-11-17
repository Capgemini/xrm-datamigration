using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Capgemini.DataMigration.Core;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.FileStore.DataStore;
using Capgemini.Xrm.DataMigration.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.FileStore.UnitTests.DataStore
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DataFileStoreWriterCsvTest
    {
        private readonly string extractPath = Path.Combine(TestBase.GetWorkiongFolderPath(), "DataExtractTest");

        [TestMethod]
        [TestCategory(TestBase.AutomatedTestCategory)]
        public void SaveBatchDataToStoreTest()
        {
            if (Directory.Exists(extractPath))
            {
                Directory.Delete(extractPath, true);
            }

            Directory.CreateDirectory(extractPath);

            CrmSchemaConfiguration schemaConfig = GetSchema();

            DataFileStoreWriterCsv store = new DataFileStoreWriterCsv(new ConsoleLogger(), "test", extractPath, null, schemaConfig);

            List<EntityWrapper> entities = PrepareEntities();

            FluentActions.Invoking(() => store.SaveBatchDataToStore(entities))
                .Should()
                .NotThrow();
        }

        private static Entity CreateContact(string firstName, string lastName)
        {
            Entity cont1 = new Entity("contact")
            {
                Id = Guid.NewGuid()
            };
            cont1.Attributes["firstname"] = firstName;
            cont1.Attributes["lastname"] = lastName;
            return cont1;
        }

        private static List<EntityWrapper> PrepareEntities()
        {
            List<EntityWrapper> entities = new List<EntityWrapper>
            {
                new EntityWrapper(CreateContact("Firs Test 1", "Last Test 1"), false),
                new EntityWrapper(CreateContact("Firs Test 2", "Last Test 2"), false),
                new EntityWrapper(CreateContact("Firs Test 3", "Last Test 3"), false),
                new EntityWrapper(CreateContact("Firs Test 4", "Last Test 4"), false)
            };
            return entities;
        }

        private static CrmSchemaConfiguration GetSchema()
        {
            CrmSchemaConfiguration schemaConfig = new CrmSchemaConfiguration();
            List<CrmEntity> entities = new List<CrmEntity>();
            CrmEntity contact = new CrmEntity
            {
                Name = "contact",
                PrimaryIdField = "contactid"
            };

            List<CrmField> fields = new List<CrmField>
            {
                new CrmField { FieldName = "contactid", FieldType = "guid" },
                new CrmField { FieldName = "firstname", FieldType = "string" },
                new CrmField { FieldName = "lastname", FieldType = "string" }
            };
            contact.CrmFields.AddRange(fields);

            entities.Add(contact);
            schemaConfig.Entities.AddRange(entities);

            return schemaConfig;
        }
    }
}