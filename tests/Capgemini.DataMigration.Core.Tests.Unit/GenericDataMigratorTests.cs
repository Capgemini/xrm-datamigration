using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.DataMigration.DataStore;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Moq;

namespace Capgemini.DataMigration.Core.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class GenericDataMigratorTests : UnitTestBase
    {
        private GenericDataMigrator<Entity, MigrationEntityWrapper<Entity>> systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();

            systemUnderTest = new GenericDataMigrator<Entity, MigrationEntityWrapper<Entity>>(MockLogger.Object, MockDataStoreReader.Object, MockDataStoreWriter.Object);
        }

        [TestMethod]
        public void GenericDataMigrator()
        {
            FluentActions.Invoking(() => new GenericDataMigrator<Entity, MigrationEntityWrapper<Entity>>(MockLogger.Object, MockDataStoreReader.Object, MockDataStoreWriter.Object))
                       .Should()
                       .NotThrow();
        }

        [TestMethod]
        public void GenericDataMigratorWithCancellation()
        {
            FluentActions.Invoking(() => new GenericDataMigrator<Entity, MigrationEntityWrapper<Entity>>(MockLogger.Object, MockDataStoreReader.Object, MockDataStoreWriter.Object, CancellationToken.None))
                      .Should()
                      .NotThrow();
        }

        [TestMethod]
        public void AddProcessor()
        {
            FluentActions.Invoking(() => systemUnderTest.AddProcessor(MockProcessor.Object))
                        .Should()
                        .NotThrow();

            MockLogger.Verify(a => a.LogVerbose(It.IsAny<string>()));
        }

        [TestMethod]
        public void MigrateDataWithNoDataFromDataStore()
        {
            var data = new List<MigrationEntityWrapper<Entity>>
            {
            };

            MockDataStoreWriter.Setup(a => a.Reset());
            MockDataStoreReader.Setup(a => a.Reset());
            MockDataStoreReader.SetupSequence(a => a.ReadBatchDataFromStore())
                               .Returns(data);

            FluentActions.Invoking(() => systemUnderTest.MigrateData())
                        .Should()
                        .NotThrow();

            MockLogger.Verify(a => a.LogInfo(It.IsAny<string>()), Times.AtLeast(5));
            MockDataStoreWriter.VerifyAll();
            MockDataStoreReader.VerifyAll();
        }

        [TestMethod]
        public void MigrateDataWithDataFromDataStore()
        {
            var data = new List<MigrationEntityWrapper<Entity>>
            {
                new TestMigrationEntityWrapper(new Entity())
            };

            MockDataStoreWriter.Setup(a => a.Reset());
            MockDataStoreWriter.Setup(a => a.SaveBatchDataToStore(It.IsAny<List<MigrationEntityWrapper<Entity>>>()));
            MockDataStoreReader.Setup(a => a.Reset());
            MockDataStoreReader.SetupSequence(a => a.ReadBatchDataFromStore())
                               .Returns(data)
                               .Returns(new List<MigrationEntityWrapper<Entity>>());

            FluentActions.Invoking(() => systemUnderTest.MigrateData())
                        .Should()
                        .NotThrow();

            MockLogger.Verify(a => a.LogInfo(It.IsAny<string>()), Times.AtLeast(5));
            MockDataStoreWriter.VerifyAll();
            MockDataStoreReader.VerifyAll();
        }

        [TestMethod]
        public void GetStartingPassNumber()
        {
            var actual = systemUnderTest.GetStartingPassNumber();

            actual.Should().Be(1);
        }
    }
}