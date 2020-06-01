using System.Diagnostics.CodeAnalysis;
using Capgemini.Xrm.DataMigration.FileStore.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.FileStore.UnitTests.DataStore
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class FileStoreReaderConfigTests
    {
        private FileStoreReaderConfig systemUnderTest;

        [TestMethod]
        public void PropertiesInitializedToDefaultValues()
        {
            systemUnderTest = new FileStoreReaderConfig();

            Assert.AreEqual("ExportedData", systemUnderTest.FilePrefix);
            Assert.IsNull(systemUnderTest.JsonFolderPath);
        }
    }
}