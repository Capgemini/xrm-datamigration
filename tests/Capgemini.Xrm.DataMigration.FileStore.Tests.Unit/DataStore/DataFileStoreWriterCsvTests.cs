using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.FileStore.UnitTests;
using Capgemini.Xrm.DataMigration.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.FileStore.DataStore.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DataFileStoreWriterCsvTests : UnitTestBase
    {
        private CrmSchemaConfiguration crmSchemaConfiguration;

        private DataFileStoreWriterCsv systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
            var entity = new CrmEntity
            {
                Name = "contact",
                PrimaryIdField = "contactid"
            };
            crmSchemaConfiguration = new CrmSchemaConfiguration();
            crmSchemaConfiguration.Entities.Add(entity);

            systemUnderTest = new DataFileStoreWriterCsv(MockLogger.Object, FilePrefix, TestResultFolder, ExcludedFields, crmSchemaConfiguration);
        }

        [TestMethod]
        public void DataFileStoreWriterCsvWithNullCrmSchemaConfiguration()
        {
            MockFileStoreWriterConfig.SetupGet(a => a.FilePrefix).Returns(FilePrefix);
            MockFileStoreWriterConfig.SetupGet(a => a.JsonFolderPath).Returns(TestResultFolder);
            MockFileStoreWriterConfig.SetupGet(a => a.ExcludedFields).Returns(ExcludedFields);

            FluentActions.Invoking(() => new DataFileStoreWriterCsv(MockLogger.Object, MockFileStoreWriterConfig.Object, null))
                              .Should()
                              .Throw<ArgumentNullException>();

            MockFileStoreWriterConfig.VerifyAll();
        }

        [TestMethod]
        public void DataFileStoreWriterCsvWithEmptyCrmSchemaConfiguration()
        {
            MockFileStoreWriterConfig.SetupGet(a => a.FilePrefix).Returns(FilePrefix);
            MockFileStoreWriterConfig.SetupGet(a => a.JsonFolderPath).Returns(TestResultFolder);
            MockFileStoreWriterConfig.SetupGet(a => a.ExcludedFields).Returns(ExcludedFields);

            FluentActions.Invoking(() => new DataFileStoreWriterCsv(MockLogger.Object, MockFileStoreWriterConfig.Object, new CrmSchemaConfiguration()))
                              .Should()
                              .Throw<ArgumentNullException>();

            MockFileStoreWriterConfig.VerifyAll();
        }

        [TestMethod]
        public void DataFileStoreWriterCsv()
        {
            MockFileStoreWriterConfig.SetupGet(a => a.FilePrefix).Returns(FilePrefix);
            MockFileStoreWriterConfig.SetupGet(a => a.JsonFolderPath).Returns(TestResultFolder);
            MockFileStoreWriterConfig.SetupGet(a => a.ExcludedFields).Returns(ExcludedFields);

            FluentActions.Invoking(() => new DataFileStoreWriterCsv(MockLogger.Object, MockFileStoreWriterConfig.Object, crmSchemaConfiguration))
                              .Should()
                              .NotThrow();

            MockFileStoreWriterConfig.VerifyAll();
        }

        [TestMethod]
        public void DataFileStoreWriterCsvSecondConstructor()
        {
            FluentActions.Invoking(() => new DataFileStoreWriterCsv(MockLogger.Object, FilePrefix, TestResultFolder, ExcludedFields, crmSchemaConfiguration))
                             .Should()
                             .NotThrow();
        }

        [TestMethod]
        public void DataFileStoreWriterCsvSecondConstructorWithNullCrmSchemaConfiguration()
        {
            FluentActions.Invoking(() => new DataFileStoreWriterCsv(MockLogger.Object, FilePrefix, TestResultFolder, ExcludedFields, null))
                             .Should()
                             .Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void DataFileStoreWriterCsvSecondConstructorWithEmptyCrmSchemaConfiguration()
        {
            FluentActions.Invoking(() => new DataFileStoreWriterCsv(MockLogger.Object, FilePrefix, TestResultFolder, ExcludedFields, new CrmSchemaConfiguration()))
                             .Should()
                             .Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Reset()
        {
            FluentActions.Invoking(() => systemUnderTest.Reset())
                             .Should()
                             .NotThrow();
        }

        [TestMethod]
        public void SaveBatchDataToStore()
        {
            var entities = new List<EntityWrapper>
            {
                new EntityWrapper(new Entity("contact", Guid.NewGuid()) { })
            };

            FluentActions.Invoking(() => systemUnderTest.SaveBatchDataToStore(entities))
                             .Should()
                             .NotThrow();
        }

        [TestMethod]
        [TestCategory(TestBase.AutomatedTestCategory)]
        public void SaveBatchDataToStoreWithSampleContact()
        {
            var schemaConfig = GetSchema();

            systemUnderTest = new DataFileStoreWriterCsv(MockLogger.Object, $"{Guid.NewGuid()}", TestResultFolder, null, schemaConfig);

            var entities = PrepareEntities();

            FluentActions.Invoking(() => systemUnderTest.SaveBatchDataToStore(entities))
                .Should()
                .NotThrow();
        }
    }
}