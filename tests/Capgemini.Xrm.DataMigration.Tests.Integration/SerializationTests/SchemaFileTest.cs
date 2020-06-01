using Capgemini.Xrm.DataMigration.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.IntegrationTests.SerializationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class SchemaFileTest
    {
        [TestMethod]
        public void ReadSchemaFromFile()
        {
            var schema = CrmSchemaConfiguration.ReadFromFile("TestData/usersettingsschema.xml");

            Assert.IsNotNull(schema);
        }
    }
}