using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

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
        public void ProcessEntityObfuscateStringFields()
        {
            // Arrange
            CrmGenericImporterConfig config = GetCrmConfigWithFieldsToObfuscate();

            string firstnameBefore = "Bob";
            string surnameBefore = "Tester";

            Entity entity = new Entity("contact");
            entity.Attributes.Add("firstname", firstnameBefore);
            entity.Attributes.Add("surname", surnameBefore);
            EntityWrapper entityWrapper = new EntityWrapper(entity);

            List<FieldToBeObfuscated> fiedlsToBeObfuscated = new List<FieldToBeObfuscated>();
            fiedlsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "firstname" });

            EntityToBeObfuscated entityToBeObfuscated = new EntityToBeObfuscated() { EntityName = "contact" };
            entityToBeObfuscated.FieldsToBeObfuscated.AddRange(fiedlsToBeObfuscated);

            var fieldToBeObfuscated = new List<EntityToBeObfuscated>();
            fieldToBeObfuscated.Add(entityToBeObfuscated);

            config.FieldsToObfuscate.AddRange(fieldToBeObfuscated);

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
        public void ProcessEntityObfuscateMultipleStringFields()
        {
            // Arrange
            CrmGenericImporterConfig config = GetCrmConfigWithFieldsToObfuscate();

            string firstnameBefore = "Tester";
            string surnameBefore = "Tester";

            Entity entity = new Entity("contact");
            entity.Attributes.Add("firstname", firstnameBefore);
            entity.Attributes.Add("surname", surnameBefore);
            EntityWrapper entityWrapper = new EntityWrapper(entity);

            List<FieldToBeObfuscated> fiedlsToBeObfuscated = new List<FieldToBeObfuscated>();
            fiedlsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "firstname" });
            fiedlsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "surname" });

            EntityToBeObfuscated entityToBeObfuscated = new EntityToBeObfuscated() { EntityName = "contact" };
            entityToBeObfuscated.FieldsToBeObfuscated.AddRange(fiedlsToBeObfuscated);

            var fieldToBeObfuscated = new List<EntityToBeObfuscated>();
            fieldToBeObfuscated.Add(entityToBeObfuscated);

            config.FieldsToObfuscate.AddRange(fieldToBeObfuscated);

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
        public void ProcessEntityObfuscateIntegerFields()
        {
            // Arrange
            CrmGenericImporterConfig config = GetCrmConfigWithFieldsToObfuscate();
            InitializeEntityMetadata();
            MockEntityMetadataCache
                .Setup(cache => cache.GetAttribute("contact", "age"))
                .Returns(new IntegerAttributeMetadata());

            int ageBefore = 25;
            string surnameBefore = "Tester";

            Entity entity = new Entity("contact");
            entity.Attributes.Add("age", ageBefore);
            entity.Attributes.Add("surname", surnameBefore);
            EntityWrapper entityWrapper = new EntityWrapper(entity);
            List<FieldToBeObfuscated> fiedlsToBeObfuscated = new List<FieldToBeObfuscated>
            {
                new FieldToBeObfuscated() { FieldName = "age" }
            };

            EntityToBeObfuscated entityToBeObfuscated = new EntityToBeObfuscated() { EntityName = "contact" };
            entityToBeObfuscated.FieldsToBeObfuscated.AddRange(fiedlsToBeObfuscated);

            var fieldToBeObfuscated = new List<EntityToBeObfuscated>
            {
                entityToBeObfuscated
            };

            config.FieldsToObfuscate.AddRange(fieldToBeObfuscated);

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
        public void ProcessEntityObfuscateMultipleIntegerFields()
        {
            // Arrange
            CrmGenericImporterConfig config = GetCrmConfigWithFieldsToObfuscate();
            InitializeEntityMetadata();
            MockEntityMetadataCache
                .Setup(cache => cache.GetAttribute("contact", "age"))
                .Returns(new IntegerAttributeMetadata());

            MockEntityMetadataCache
                .Setup(cache => cache.GetAttribute("contact", "idnumber"))
                .Returns(new IntegerAttributeMetadata());

            int ageBefore = 25;
            int idNumberBefore = 25;

            Entity entity = new Entity("contact");
            entity.Attributes.Add("age", ageBefore);
            entity.Attributes.Add("idnumber", idNumberBefore);
            EntityWrapper entityWrapper = new EntityWrapper(entity);
            List<FieldToBeObfuscated> fiedlsToBeObfuscated = new List<FieldToBeObfuscated>
            {
                new FieldToBeObfuscated() { FieldName = "age" },
                new FieldToBeObfuscated() { FieldName = "idnumber" }
            };

            EntityToBeObfuscated entityToBeObfuscated = new EntityToBeObfuscated() { EntityName = "contact" };
            entityToBeObfuscated.FieldsToBeObfuscated.AddRange(fiedlsToBeObfuscated);

            var fieldToBeObfuscated = new List<EntityToBeObfuscated>();
            fieldToBeObfuscated.Add(entityToBeObfuscated);

            config.FieldsToObfuscate.AddRange(fieldToBeObfuscated);

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
        public void ProcessEntityObfuscateDoubleFields()
        {
            // Arrange
            CrmGenericImporterConfig config = GetCrmConfigWithFieldsToObfuscate();
            InitializeEntityMetadata();
            MockEntityMetadataCache
                .Setup(cache => cache.GetAttribute("contact", "address1_latitude"))
                .Returns(new DoubleAttributeMetadata());

            double latitudeBefore = 51.5178737;

            Entity entity = new Entity("contact");
            entity.Attributes.Add("address1_latitude", latitudeBefore);
            EntityWrapper entityWrapper = new EntityWrapper(entity);

            List<FieldToBeObfuscated> fieldsToBeObfuscated = new List<FieldToBeObfuscated>();
            fieldsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "address1_latitude" });

            EntityToBeObfuscated entityToBeObfuscated = new EntityToBeObfuscated() { EntityName = "contact" };
            entityToBeObfuscated.FieldsToBeObfuscated.AddRange(fieldsToBeObfuscated);

            var fieldToBeObfuscated = new List<EntityToBeObfuscated>();
            fieldToBeObfuscated.Add(entityToBeObfuscated);

            config.FieldsToObfuscate.AddRange(fieldToBeObfuscated);

            // Act
            ObfuscateFieldsProcessor processor = new ObfuscateFieldsProcessor(MockEntityMetadataCache.Object, config.FieldsToObfuscate);
            processor.ProcessEntity(entityWrapper, 1, 1);

            double latitudeAfter = (double)entity["address1_latitude"];

            // Assert
            Assert.AreNotEqual(latitudeBefore, latitudeAfter);
        }

        [TestMethod]
        public void ProcessEntityObfuscateDecimalFields()
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
            List<FieldToBeObfuscated> fiedlsToBeObfuscated = new List<FieldToBeObfuscated>();
            fiedlsToBeObfuscated.Add(new FieldToBeObfuscated() { FieldName = "creditlimit" });
            EntityToBeObfuscated entityToBeObfuscated = new EntityToBeObfuscated() { EntityName = "contact" };
            entityToBeObfuscated.FieldsToBeObfuscated.AddRange(fiedlsToBeObfuscated);
            var fieldToBeObfuscated = new List<EntityToBeObfuscated>();
            fieldToBeObfuscated.Add(entityToBeObfuscated);

            config.FieldsToObfuscate.AddRange(fieldToBeObfuscated);

            // Act
            ObfuscateFieldsProcessor processor = new ObfuscateFieldsProcessor(MockEntityMetadataCache.Object, config.FieldsToObfuscate);
            processor.ProcessEntity(entityWrapper, 1, 1);

            decimal creditLimitAfter = (decimal)entity["creditlimit"];

            // Assert
            Assert.AreNotEqual(creditLimitBefore, creditLimitAfter);
        }

        private CrmGenericImporterConfig GetCrmConfigWithFieldsToObfuscate()
        {
            var config = new CrmGenericImporterConfig();

            return config;
        }
    }
}