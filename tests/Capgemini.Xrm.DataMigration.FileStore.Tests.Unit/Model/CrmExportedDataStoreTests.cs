using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.FileStore.Model.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmExportedDataStoreTests
    {
        [TestMethod]
        public void CrmExportedDataStore()
        {
            CrmExportedDataStore systemUnderTest = null;

            FluentActions.Invoking(() => systemUnderTest = new CrmExportedDataStore())
                .Should()
                .NotThrow();

            Assert.IsTrue(systemUnderTest.ExportedEntities.Count == 0);
            Assert.IsTrue(systemUnderTest.RecordsCount == 0);
        }
    }
}