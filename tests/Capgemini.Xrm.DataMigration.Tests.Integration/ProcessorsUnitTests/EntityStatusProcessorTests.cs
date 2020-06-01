using System.Collections.Generic;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using Capgemini.Xrm.DataMigration.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.IntegrationTests.ProcessorsUnitTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class EntityStatusProcessorTests
    {
        [TestMethod]
        public void ProcessEntityIgnoreNoException()
        {
            bool ignoreStatus = true;
            string entityName = "myEntity";
            List<string> exceptions = new List<string>() { "someentity1", "entity with tpyo inname" };

            Entity entity = new Entity()
            {
                LogicalName = entityName
            };
            entity.Attributes.Add(EntityStatusProcessor.StatusFields.ElementAt(0), new OptionSetValue(2));
            entity.Attributes.Add(EntityStatusProcessor.StatusFields.ElementAt(1), new OptionSetValue(4));

            EntityWrapper entityWrapper = new EntityWrapper(entity);

            EntityStatusProcessor processor = new EntityStatusProcessor(ignoreStatus, exceptions);

            processor.ProcessEntity(entityWrapper, (int)PassType.SetRecordStatus, (int)PassType.SetRecordStatus);

            Assert.AreEqual(OperationType.Ignore, entityWrapper.OperationType);
        }

        [TestMethod]
        public void ProcessEntityIgnoreInExceptions()
        {
            bool ignoreStatus = true;
            string entityName = "myEntity";
            List<string> exceptions = new List<string>() { "someentity1", entityName, "entity with tpyo inname" };

            Entity entity = new Entity()
            {
                LogicalName = entityName
            };
            entity.Attributes.Add(EntityStatusProcessor.StatusFields.ElementAt(0), new OptionSetValue(2));
            entity.Attributes.Add(EntityStatusProcessor.StatusFields.ElementAt(1), new OptionSetValue(4));

            EntityWrapper entityWrapper = new EntityWrapper(entity);

            EntityStatusProcessor processor = new EntityStatusProcessor(ignoreStatus, exceptions);

            processor.ProcessEntity(entityWrapper, (int)PassType.SetRecordStatus, (int)PassType.SetRecordStatus);

            Assert.IsTrue(entity.Contains(EntityStatusProcessor.StatusFields.ElementAt(0)));
            Assert.IsTrue(entity.Contains(EntityStatusProcessor.StatusFields.ElementAt(1)));
        }

        [TestMethod]
        public void ProcessEntityIncludeNoException()
        {
            bool ignoreStatus = false;
            string entityName = "myEntity";
            List<string> exceptions = new List<string>() { "someentity1", "entity with tpyo inname" };

            Entity entity = new Entity()
            {
                LogicalName = entityName
            };
            entity.Attributes.Add(EntityStatusProcessor.StatusFields.ElementAt(0), new OptionSetValue(2));
            entity.Attributes.Add(EntityStatusProcessor.StatusFields.ElementAt(1), new OptionSetValue(4));

            EntityWrapper entityWrapper = new EntityWrapper(entity);

            EntityStatusProcessor processor = new EntityStatusProcessor(ignoreStatus, exceptions);

            processor.ProcessEntity(entityWrapper, (int)PassType.SetRecordStatus, (int)PassType.SetRecordStatus);

            Assert.IsTrue(entity.Contains(EntityStatusProcessor.StatusFields.ElementAt(0)));
            Assert.IsTrue(entity.Contains(EntityStatusProcessor.StatusFields.ElementAt(1)));
        }

        [TestMethod]
        public void ProcessEntityIncludeInExceptions()
        {
            bool ignoreStatus = false;
            string entityName = "myEntity";
            List<string> exceptions = new List<string>() { "someentity1", entityName, "entity with tpyo inname" };

            Entity entity = new Entity()
            {
                LogicalName = entityName
            };
            entity.Attributes.Add(EntityStatusProcessor.StatusFields.ElementAt(0), new OptionSetValue(2));
            entity.Attributes.Add(EntityStatusProcessor.StatusFields.ElementAt(1), new OptionSetValue(4));

            EntityWrapper entityWrapper = new EntityWrapper(entity);

            EntityStatusProcessor processor = new EntityStatusProcessor(ignoreStatus, exceptions);

            processor.ProcessEntity(entityWrapper, (int)PassType.SetRecordStatus, (int)PassType.SetRecordStatus);

            Assert.AreEqual(OperationType.Ignore, entityWrapper.OperationType);
        }

        [TestMethod]
        public void ProcessEntityRemovedOnOtherPasses()
        {
            bool ignoreStatus = false;
            string entityName = "myEntity";
            List<string> exceptions = new List<string>() { "someentity1", entityName, "entity with tpyo inname" };

            Entity entity = new Entity()
            {
                LogicalName = entityName
            };
            entity.Attributes.Add(EntityStatusProcessor.StatusFields.ElementAt(0), new OptionSetValue(2));
            entity.Attributes.Add(EntityStatusProcessor.StatusFields.ElementAt(1), new OptionSetValue(4));

            EntityWrapper entityWrapper = new EntityWrapper(entity);

            EntityStatusProcessor processor = new EntityStatusProcessor(ignoreStatus, exceptions);

            processor.ProcessEntity(entityWrapper, (int)PassType.SetRecordStatus + 1, (int)PassType.SetRecordStatus);

            Assert.IsFalse(entity.Contains(EntityStatusProcessor.StatusFields.ElementAt(0)));
            Assert.IsFalse(entity.Contains(EntityStatusProcessor.StatusFields.ElementAt(1)));
        }
    }
}