using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Capgemini.DataMigration.Core.Tests.Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using Capgemini.Xrm.DataMigration.CrmStore.Config;


namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.DataProcessors
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ObfuscateFieldsProcessorTests : UnitTestBase
    {
        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
        }

        [TestMethod]
        public void ProcessEntity_ObfuscateStringFields()
        {
            // Arrange
            CrmGenericImporterConfig config = GetCrmConfigWithFieldsToObfuscate();

            string firstnameBefore = "Bob";
            string surnameBefore = "Tester";

            Entity entity = new Entity("Contact");
            entity.Attributes.Add("firstname", firstnameBefore);
            entity.Attributes.Add("surname", surnameBefore);
            EntityWrapper entityWrapper = new EntityWrapper(entity);

            config.FieldsToObfuscate = new Dictionary<string, List<string>>()
            {
                { "Contact", new List<string>() { "firstname" } }
            };

            // Act
            ObfuscateFieldsProcessor processor = new ObfuscateFieldsProcessor(MockEntityMetadataCache.Object, config.FieldsToObfuscate);
            processor.ProcessEntity(entityWrapper, 1, 1);

            string firstnameAfter = (string)entity["firstname"];
            string surnameAfter = (string)entity["surname"];

            // Assert
            Assert.AreNotEqual(firstnameBefore, firstnameAfter);
            Assert.AreEqual(surnameBefore, surnameAfter);

        }

        [TestMethod]
        public void ProcessEntity_ObfuscateMultipleStringFields()
        {
            // Arrange
            CrmGenericImporterConfig config = GetCrmConfigWithFieldsToObfuscate();

            string firstnameBefore = "Tester";
            string surnameBefore = "Tester";

            Entity entity = new Entity("Contact");
            entity.Attributes.Add("firstname", firstnameBefore);
            entity.Attributes.Add("surname", surnameBefore);
            EntityWrapper entityWrapper = new EntityWrapper(entity);

            config.FieldsToObfuscate = new Dictionary<string, List<string>>()
            {
                { "Contact", new List<string>() { "firstname", "surname" } }
            };

            // Act
            ObfuscateFieldsProcessor processor = new ObfuscateFieldsProcessor(MockEntityMetadataCache.Object, config.FieldsToObfuscate);
            processor.ProcessEntity(entityWrapper, 1, 1);

            string firstnameAfter = (string)entity["firstname"];
            string surnameAfter = (string)entity["surname"];

            // Assert
            Assert.AreNotEqual(firstnameBefore, firstnameAfter);
            Assert.AreNotEqual(surnameBefore, surnameAfter);
            Assert.AreNotEqual(firstnameAfter, surnameAfter);
        }

        [TestMethod]
        public void ProcessEntity_ObfuscateIntegerFields()
        {
            // Arrange
            CrmGenericImporterConfig config = GetCrmConfigWithFieldsToObfuscate();
            InitializeEntityMetadata();
            MockEntityMetadataCache
                .Setup(cache => cache.GetAttribute("Contact", "age"))
                .Returns(new IntegerAttributeMetadata());

            int ageBefore = 25;
            string surnameBefore = "Tester";

            Entity entity = new Entity("Contact");
            entity.Attributes.Add("age", ageBefore);
            entity.Attributes.Add("surname", surnameBefore);
            EntityWrapper entityWrapper = new EntityWrapper(entity);

            config.FieldsToObfuscate = new Dictionary<string, List<string>>()
            {
                { "Contact", new List<string>() { "age" } }
            };

            // Act
            ObfuscateFieldsProcessor processor = new ObfuscateFieldsProcessor(MockEntityMetadataCache.Object, config.FieldsToObfuscate);
            processor.ProcessEntity(entityWrapper, 1, 1);

            int ageAfter = (int)entity["age"];
            string surnameAfter = (string)entity["surname"];

            // Assert
            Assert.AreNotEqual(ageBefore, ageAfter);
            Assert.AreEqual(surnameBefore, surnameAfter);

        }

        [TestMethod]
        public void ProcessEntity_ObfuscateMultipleIntegerFields()
        {
            // Arrange
            CrmGenericImporterConfig config = GetCrmConfigWithFieldsToObfuscate();
            InitializeEntityMetadata();
            MockEntityMetadataCache
                .Setup(cache => cache.GetAttribute("Contact", "age"))
                .Returns(new IntegerAttributeMetadata());

            MockEntityMetadataCache
                .Setup(cache => cache.GetAttribute("Contact", "idnumber"))
                .Returns(new IntegerAttributeMetadata());

            int ageBefore = 25;
            int idNumberBefore = 25;

            Entity entity = new Entity("Contact");
            entity.Attributes.Add("age", ageBefore);
            entity.Attributes.Add("idnumber", idNumberBefore);
            EntityWrapper entityWrapper = new EntityWrapper(entity);

            config.FieldsToObfuscate = new Dictionary<string, List<string>>()
            {
                { "Contact", new List<string>() { "age", "idnumber" } }
            };

            // Act
            ObfuscateFieldsProcessor processor = new ObfuscateFieldsProcessor(MockEntityMetadataCache.Object, config.FieldsToObfuscate);
            processor.ProcessEntity(entityWrapper, 1, 1);

            int ageAfter = (int)entity["age"];
            int idNumberAfter = (int)entity["idnumber"];

            // Assert
            Assert.AreNotEqual(ageBefore, ageAfter);
            Assert.AreNotEqual(idNumberBefore, idNumberAfter);
        }

        [TestMethod]
        public void ProcessEntity_ObfuscateDoubleFields()
        {
            // Arrange
            CrmGenericImporterConfig config = GetCrmConfigWithFieldsToObfuscate();
            InitializeEntityMetadata();
            MockEntityMetadataCache
                .Setup(cache => cache.GetAttribute("Contact", "address1_latitude"))
                .Returns(new DoubleAttributeMetadata());

            double latitudeBefore = 51.5178737;

            Entity entity = new Entity("Contact");
            entity.Attributes.Add("address1_latitude", latitudeBefore);
            EntityWrapper entityWrapper = new EntityWrapper(entity);

            config.FieldsToObfuscate = new Dictionary<string, List<string>>()
            {
                { "Contact", new List<string>() { "address1_latitude" } }
            };

            // Act
            ObfuscateFieldsProcessor processor = new ObfuscateFieldsProcessor(MockEntityMetadataCache.Object, config.FieldsToObfuscate);
            processor.ProcessEntity(entityWrapper, 1, 1);

            double latitudeAfter = (double)entity["address1_latitude"];

            // Assert
            Assert.AreNotEqual(latitudeBefore, latitudeAfter);
        }

        [TestMethod]
        public void ProcessEntity_ObfuscateDecimalFields()
        {
            // Arrange
            CrmGenericImporterConfig config = GetCrmConfigWithFieldsToObfuscate();
            InitializeEntityMetadata();
            MockEntityMetadataCache
                .Setup(cache => cache.GetAttribute("contact", "creditlimit"))
                .Returns(new DecimalAttributeMetadata());

            decimal creditLimitBefore = 1000M;

            Entity entity = new Entity("contact");
            entity.Attributes.Add("creditlimit", creditLimitBefore);
            EntityWrapper entityWrapper = new EntityWrapper(entity);

            config.FieldsToObfuscate = new Dictionary<string, List<string>>()
            {
                { "contact", new List<string>() { "creditlimit" } }
            };

            // Act
            ObfuscateFieldsProcessor processor = new ObfuscateFieldsProcessor(MockEntityMetadataCache.Object, config.FieldsToObfuscate);
            processor.ProcessEntity(entityWrapper, 1, 1);

            decimal creditLimitAfter = (decimal)entity["creditlimit"];

            // Assert
            Assert.AreNotEqual(creditLimitBefore, creditLimitAfter);
        }

        private CrmGenericImporterConfig GetCrmConfigWithFieldsToObfuscate()
        {
            CrmGenericImporterConfig config = new CrmGenericImporterConfig();
            config.FieldsToObfuscate = new Dictionary<string, List<string>>();

            return config;
        }

    }
}
