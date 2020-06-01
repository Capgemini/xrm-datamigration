using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.FileStore.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public abstract class TestBase
    {
        public const string AutomatedTestCategory = "AutomatedTest";

        public TestContext TestContext { get; set; }

        public static string GetWorkiongFolderPath()
        {
            string folderPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            return folderPath;
        }
    }
}