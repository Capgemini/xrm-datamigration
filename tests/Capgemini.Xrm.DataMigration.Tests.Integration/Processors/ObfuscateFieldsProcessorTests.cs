using Capgemini.Xrm.DataMigration.Cache;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capgemini.Xrm.DataMigration.IntegrationTests.Processors
{
    [TestClass]
    public class ObfuscateFieldsProcessorTests
    {
        [TestMethod]
        public void ObfuscateStringFieldsTest()
        {

            var orgService = ConnectionHelper.GetOrganizationalServiceTarget();
            var cache = new EntityMetadataCache(orgService);

            Dictionary<string, List<string>> fieldsToObfuscate = new Dictionary<string, List<string>>();
            fieldsToObfuscate.Add("contact", new List<string>() { "firstname" });

            ObfuscateFieldsProcessor processor = new ObfuscateFieldsProcessor(cache, fieldsToObfuscate);

            string beforeFirstName = "Bob";
            string beforeLastName = "test";

            Entity ent = new Entity("contact");
            ent.Attributes.Add("firstname", beforeFirstName);
            ent.Attributes.Add("lastname", beforeLastName);

            EntityWrapper entWrap = new EntityWrapper(ent);

            processor.ProcessEntity(entWrap, 1, 1);

            Assert.AreNotEqual(beforeFirstName, entWrap.OriginalEntity.Attributes["firstname"]);
            Assert.AreEqual(beforeLastName, entWrap.OriginalEntity.Attributes["lastname"]);

        }

        [TestMethod]
        public void ObfuscateIntFieldsTest()
        {

            var orgService = ConnectionHelper.GetOrganizationalServiceTarget();
            var cache = new EntityMetadataCache(orgService);

            Dictionary<string, List<string>> fieldsToObfuscate = new Dictionary<string, List<string>>();
            fieldsToObfuscate.Add("contact", new List<string>() { "numberofchildren" });

            ObfuscateFieldsProcessor processor = new ObfuscateFieldsProcessor(cache, fieldsToObfuscate);

            string beforeFirstName = "Bob";
            int beforeNumberOfChildren = 5;

            Entity ent = new Entity("contact");
            ent.Attributes.Add("firstname", beforeFirstName);
            ent.Attributes.Add("numberofchildren", beforeNumberOfChildren);

            EntityWrapper entWrap = new EntityWrapper(ent);

            processor.ProcessEntity(entWrap, 1, 1);

            Assert.AreEqual(beforeFirstName, entWrap.OriginalEntity.Attributes["firstname"]);
            Assert.AreNotEqual(beforeNumberOfChildren, entWrap.OriginalEntity.Attributes["numberofchildren"]);

        }
    }
}
