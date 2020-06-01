using System.Diagnostics.CodeAnalysis;
using Capgemini.Xrm.DataMigration.FileStore.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.FileStore.UnitTests.DataStore
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class FileStoreWriterConfigTests
    {
        private FileStoreWriterConfig systemUnderTest;

        [TestMethod]
        public void PropertiesInitializedToDefaultValues()
        {
            systemUnderTest = new FileStoreWriterConfig();

            Assert.IsTrue(systemUnderTest.SeperateFilesPerEntity);
            Assert.IsTrue(systemUnderTest.ExcludedFields.Count == 0);
        }
    }
}