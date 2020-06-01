using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.FileStore.DataStore;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.IntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class DataFileStoreReaderWriterTest
    {
        [TestMethod]
        public void SaveBatchDataToStore()
        {
            DataFileStoreWriter mgr = new DataFileStoreWriter(new ConsoleLogger(), $"{DateTime.UtcNow.ToString("yyMMmmss", CultureInfo.InvariantCulture)}testexport", "TestData");

            FluentActions.Invoking(() => mgr.SaveBatchDataToStore(EntityMockHelper.EntitiesToCreate.Select(p => new EntityWrapper(p)).ToList()))
                  .Should()
                  .NotThrow();
        }

        [TestMethod]
        public void ReadBatchDataFromStore()
        {
            DataFileStoreReader mgr = new DataFileStoreReader(new ConsoleLogger(), "testexport", @"TestData");
            List<EntityWrapper> result = mgr.ReadBatchDataFromStore();

            Assert.IsTrue(result.Count > 0);
        }
    }
}