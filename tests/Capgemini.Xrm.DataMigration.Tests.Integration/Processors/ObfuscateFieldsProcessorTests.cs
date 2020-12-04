using System.Collections.Generic;
using Capgemini.DataMigration.Core.Model;
using Capgemini.Xrm.DataMigration.Cache;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

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

            List<FieldToBeObfuscated> fiedlsToBeObfuscated = new List<FieldToBeObfuscated>
            {
                new FieldToBeObfuscated() { FieldName = "firstname" }
            };

            EntityToBeObfuscated entityToBeObfuscated = new EntityToBeObfuscated() { EntityName = "contact" };
            entityToBeObfuscated.FieldsToBeObfuscated.AddRange(fiedlsToBeObfuscated);

            var fieldsToBeObfuscated = new List<EntityToBeObfuscated>
            {
                entityToBeObfuscated
            };

            ObfuscateFieldsProcessor processor = new ObfuscateFieldsProcessor(cache, fieldsToBeObfuscated);

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

            List<FieldToBeObfuscated> fiedlsToBeObfuscated = new List<FieldToBeObfuscated>
            {
                new FieldToBeObfuscated() { FieldName = "numberofchildren" }
            };

            EntityToBeObfuscated entityToBeObfuscated = new EntityToBeObfuscated() { EntityName = "contact" };
            entityToBeObfuscated.FieldsToBeObfuscated.AddRange(fiedlsToBeObfuscated);
            var fieldsToBeObfuscated = new List<EntityToBeObfuscated>
            {
                entityToBeObfuscated
            };

            ObfuscateFieldsProcessor processor = new ObfuscateFieldsProcessor(cache, fieldsToBeObfuscated);

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