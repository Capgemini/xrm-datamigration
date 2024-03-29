﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.FileStore.Model;
using Capgemini.Xrm.DataMigration.FileStore.UnitTests;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.FileStore.DataStore.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DataFileStoreWriterTests : UnitTestBase
    {
        private DataFileStoreWriter systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();

            List<string> excludedFields = new List<string>();
            bool seperateFilesPerEntity = true;

            systemUnderTest = new DataFileStoreWriter(MockLogger.Object, FilePrefix, TestResultFolder, excludedFields, seperateFilesPerEntity);
        }

        [TestMethod]
        public void DataFileStoreWriter()
        {
            MockFileStoreWriterConfig.SetupGet(a => a.SeperateFilesPerEntity).Returns(true);
            MockFileStoreWriterConfig.SetupGet(a => a.FilePrefix).Returns(FilePrefix);
            MockFileStoreWriterConfig.SetupGet(a => a.JsonFolderPath).Returns(TestResultFolder);

            FluentActions.Invoking(() => systemUnderTest = new DataFileStoreWriter(MockLogger.Object, MockFileStoreWriterConfig.Object))
                             .Should()
                             .NotThrow();

            MockFileStoreWriterConfig.VerifyAll();
        }

        [TestMethod]
        public void DataFileStoreWriterWithParameters()
        {
            List<string> excludedFields = new List<string>();
            bool seperateFilesPerEntity = true;

            FluentActions.Invoking(() => new DataFileStoreWriter(MockLogger.Object, FilePrefix, TestResultFolder, excludedFields, seperateFilesPerEntity))
                             .Should()
                             .NotThrow();
        }

        [TestMethod]
        public void DataFileStoreWriterWithNullLogger()
        {
            List<string> excludedFields = new List<string>();
            bool seperateFilesPerEntity = true;
            ILogger logger = null;

            FluentActions.Invoking(() => new DataFileStoreWriter(logger, FilePrefix, TestResultFolder, excludedFields, seperateFilesPerEntity))
                             .Should()
                             .Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void DataFileStoreWriterWithNullFilePrefix()
        {
            string filePrefix = null;
            List<string> excludedFields = new List<string>();
            bool seperateFilesPerEntity = true;

            FluentActions.Invoking(() => new DataFileStoreWriter(MockLogger.Object, filePrefix, TestResultFolder, excludedFields, seperateFilesPerEntity))
                             .Should()
                             .Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void DataFileStoreWriterWithNullFilePath()
        {
            TestResultFolder = null;
            List<string> excludedFields = new List<string>();
            bool seperateFilesPerEntity = true;

            FluentActions.Invoking(() => new DataFileStoreWriter(MockLogger.Object, FilePrefix, TestResultFolder, excludedFields, seperateFilesPerEntity))
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
        [TestCategory(TestBase.AutomatedTestCategory)]
        public void SaveBatchDataToStore()
        {
            List<EntityWrapper> entities = PrepareEntities();

            FluentActions.Invoking(() => systemUnderTest.SaveBatchDataToStore(entities))
                .Should()
                .NotThrow();
        }

        [TestMethod]
        public void RemoveEntityReferenceNamePropertyValue()
        {
            // Arrange
            Guid accountId = Guid.NewGuid();
            string accountName = "ABC Ltd";

            Entity account = new Entity("account")
            {
                Id = accountId
            };
            account.Attributes.Add("name", "Fake Account");

            EntityReference entityRef = account.ToEntityReference();
            entityRef.Name = accountName;

            Entity entity = new Entity("contact");
            entity.Attributes.Add("firstname", "Test");
            entity.Attributes.Add("surname", "Tester");
            entity.Attributes.Add("account", entityRef);

            List<FieldToBeObfuscated> fiedlsToBeObfuscated = new List<FieldToBeObfuscated>
            {
                new FieldToBeObfuscated() { FieldName = "firstname" }
            };

            var fieldToBeObfuscated = new List<EntityToBeObfuscated>();
            var entityToBeObfuscated = new EntityToBeObfuscated() { EntityName = "contact" };
            entityToBeObfuscated.FieldsToBeObfuscated.AddRange(fiedlsToBeObfuscated);
            fieldToBeObfuscated.Add(entityToBeObfuscated);

            List<EntityWrapper> entities = new List<EntityWrapper>
            {
                new EntityWrapper(entity)
            };

            var entitiesToExport = entities.Select(e => new CrmEntityStore(e)).ToList();

            var dataFileStoreWriter = new DataFileStoreWriter(MockLogger.Object, FilePrefix, TestResultFolder, null, true, fieldToBeObfuscated);

            string accountNameBefore = ((EntityReference)entity["account"]).Name;

            // Assert
            FluentActions.Invoking(() => dataFileStoreWriter.RemoveEntityReferenceNameProperty(entitiesToExport))
                                .Should()
                                .NotThrow();

            string accountNameAfter = ((EntityReference)entity["account"]).Name;
            accountNameBefore.Should().Be(accountName);
            accountNameAfter.Should().BeNull();
        }

        [TestMethod]
        public void ThrowArgumentNullExceptionIfentitiesToExportIsNull()
        {
            FluentActions.Invoking(() => systemUnderTest.RemoveEntityReferenceNameProperty(null))
                             .Should()
                             .Throw<ArgumentNullException>();
        }
    }
}