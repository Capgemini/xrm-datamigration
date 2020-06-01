using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.DataStore;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.FileStore.DataStore.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DataFileStoreWriterTests : UnitTestBase
    {
        private DataFileStoreWriter systemUnderTest = null;

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
        public void SaveBatchDataToStore()
        {
            List<EntityWrapper> entities = new List<EntityWrapper>
            {
                new EntityWrapper(new Entity("contact", Guid.NewGuid()) { })
            };

            FluentActions.Invoking(() => systemUnderTest.SaveBatchDataToStore(entities))
                             .Should()
                             .NotThrow();
        }
    }
}