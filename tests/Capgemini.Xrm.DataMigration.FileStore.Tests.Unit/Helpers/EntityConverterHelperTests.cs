using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Capgemini.DataMigration.Exceptions;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.FileStore.Helpers;
using Capgemini.Xrm.DataMigration.FileStore.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.FileStore.Tests.Unit.Helpers
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class EntityConverterHelperTests
    {
        [TestMethod]
        public void ConvertEntities()
        {
            var entityName = "account";
            var guidList = new List<Guid>();
            var entitylist = new List<CrmEntityStore>();

            for (int i = 0; i < 5; i++)
            {
                var id = Guid.NewGuid();
                guidList.Add(id);
                entitylist.Add(new CrmEntityStore(new Entity(entityName, id)));
            }

            var actual = EntityConverterHelper.ConvertEntities(entitylist);

            actual.Count.Should().Be(entitylist.Count);

            for (int i = 0; i < 5; i++)
            {
                var id = guidList[i];
                EntityWrapper item = null;
                FluentActions.Invoking(() => item = actual.First(a => a.Id.ToString() == id.ToString()))
                         .Should()
                         .NotThrow();

                item.LogicalName.Should().Be(entityName);
            }
        }

        [TestMethod]
        public void ConvertAttributes()
        {
            var attributes = new List<CrmAttributeStore>
            {
                new CrmAttributeStore(new KeyValuePair<string, object>("firstname", "Joe")),
                new CrmAttributeStore(new KeyValuePair<string, object>("lastname", "Blogs"))
            };

            var actual = EntityConverterHelper.ConvertAttributes(attributes);

            FluentActions.Invoking(() => actual.Single(a => a.Key == "firstname" && a.Value.ToString() == "Joe"))
                .Should()
                .NotThrow();
            FluentActions.Invoking(() => actual.Single(a => a.Key == "lastname" && a.Value.ToString() == "Blogs"))
                .Should()
                .NotThrow();
            actual.Count.Should().Be(attributes.Count);
        }

        [TestMethod]
        public void GetAttributeTypeForCsvString()
        {
            var actual = EntityConverterHelper.GetAttributeTypeForCsv("string");

            Assert.AreEqual(typeof(string), actual);
        }

        [TestMethod]
        public void GetAttributeTypeForCsvInteger()
        {
            var actual = EntityConverterHelper.GetAttributeTypeForCsv("integer");

            Assert.AreEqual(typeof(int), actual);
        }

        [TestMethod]
        public void GetAttributeTypeForCsvGuid()
        {
            var actual = EntityConverterHelper.GetAttributeTypeForCsv("guid");

            Assert.AreEqual(typeof(Guid), actual);
        }

        [TestMethod]
        public void GetAttributeTypeForCsvDecimal()
        {
            var actual = EntityConverterHelper.GetAttributeTypeForCsv("decimal");

            Assert.AreEqual(typeof(decimal), actual);
        }

        [TestMethod]
        public void GetAttributeTypeForCsvDouble()
        {
            var actual = EntityConverterHelper.GetAttributeTypeForCsv("double");

            Assert.AreEqual(typeof(double), actual);
        }

        [TestMethod]
        public void GetAttributeTypeForCsvEntityreference()
        {
            var actual = EntityConverterHelper.GetAttributeTypeForCsv("entityreference");

            Assert.AreEqual(typeof(Guid), actual);
        }

        [TestMethod]
        public void GetAttributeTypeForCsvOptionSetValue()
        {
            var actual = EntityConverterHelper.GetAttributeTypeForCsv("optionsetvalue");

            Assert.AreEqual(typeof(int), actual);
        }

        [TestMethod]
        public void GetAttributeTypeForCsvOptionSetValueCollection()
        {
            var actual = EntityConverterHelper.GetAttributeTypeForCsv("optionsetvaluecollection");

            Assert.AreEqual(typeof(string), actual);
        }

        [TestMethod]
        public void GetAttributeTypeForCsvBool()
        {
            var actual = EntityConverterHelper.GetAttributeTypeForCsv("bool");

            Assert.AreEqual(typeof(bool), actual);
        }

        [TestMethod]
        public void GetAttributeTypeForCsvUnSupported()
        {
            FluentActions.Invoking(() => EntityConverterHelper.GetAttributeTypeForCsv("float"))
                .Should()
                .Throw<ConfigurationException>();
        }

        [TestMethod]
        public void GetAttributeValueFromCsvNullInput()
        {
            string itemType = string.Empty;
            string lookUpType = string.Empty;
            object input = null;

            var actual = EntityConverterHelper.GetAttributeValueFromCsv(itemType, lookUpType, input);

            actual.Should().BeNull();
        }

        [TestMethod]
        public void GetAttributeValueFromCsvString()
        {
            string itemType = "string";
            string lookUpType = string.Empty;
            object input = "Joe";

            var actual = EntityConverterHelper.GetAttributeValueFromCsv(itemType, lookUpType, input);

            actual.ToString().Should().Be("Joe");
        }

        [TestMethod]
        public void GetAttributeValueFromCsvEntityreference()
        {
            var id = Guid.NewGuid();
            var entityName = "account";
            string itemType = "entityreference";
            string lookUpType = entityName;
            object input = id;

            var actual = (EntityReference)EntityConverterHelper.GetAttributeValueFromCsv(itemType, lookUpType, input);

            actual.Id.ToString().Should().Be(id.ToString());
            actual.LogicalName.Should().Be(entityName);
        }

        [TestMethod]
        public void GetAttributeValueFromCsvOptionSetValue()
        {
            string itemType = "optionsetvalue";
            string lookUpType = string.Empty;
            object input = 5;

            var actual = (OptionSetValue)EntityConverterHelper.GetAttributeValueFromCsv(itemType, lookUpType, input);

            actual.Value.Should().Be(5);
        }

        [TestMethod]
        public void GetAttributeValueFromCsvOptionSetValueCollection()
        {
            string itemType = "optionsetvaluecollection";
            string lookUpType = string.Empty;
            object input = "3|13|5";

            var actual = (OptionSetValueCollection)EntityConverterHelper.GetAttributeValueFromCsv(itemType, lookUpType, input);

            actual.Count.Should().Be(3);
            FluentActions.Invoking(() => actual.Single(a => a.Value == 13))
                 .Should()
                 .NotThrow();
            FluentActions.Invoking(() => actual.Single(a => a.Value == 3))
                 .Should()
                 .NotThrow();
            FluentActions.Invoking(() => actual.Single(a => a.Value == 5))
                 .Should()
                 .NotThrow();
        }

        [TestMethod]
        public void GetAttributeValueFromCsv()
        {
            string itemType = "integer";
            string lookUpType = string.Empty;
            object input = 5;

            var actual = (int)EntityConverterHelper.GetAttributeValueFromCsv(itemType, lookUpType, input);

            actual.Should().Be(5);
        }

        [TestMethod]
        public void GetAttributeValueForCsvEntityReference()
        {
            var input = new EntityReference("contact", Guid.NewGuid());
            CrmAttributeStore attribute = new CrmAttributeStore(new KeyValuePair<string, object>("firstname", input))
            {
                AttributeType = "Microsoft.Xrm.Sdk.EntityReference"
            };

            var actual = EntityConverterHelper.GetAttributeValueForCsv(attribute);

            actual.ToString().Should().Be(input.Id.ToString());
        }

        [TestMethod]
        public void GetAttributeValueForCsvOptionSetValue()
        {
            var input = new OptionSetValue(12);
            CrmAttributeStore attribute = new CrmAttributeStore(new KeyValuePair<string, object>("firstname", input))
            {
                AttributeType = "Microsoft.Xrm.Sdk.OptionSetValue"
            };

            var actual = EntityConverterHelper.GetAttributeValueForCsv(attribute);

            actual.ToString().Should().Be(input.Value.ToString(CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void GetAttributeValueForCsvOptionSetValueCollection()
        {
            var list = new List<OptionSetValue>
            {
                new OptionSetValue(12),
                new OptionSetValue(3)
            };
            var input = new OptionSetValueCollection(list);
            var attribute = new CrmAttributeStore(new KeyValuePair<string, object>("firstname", input))
            {
                AttributeType = "Microsoft.Xrm.Sdk.OptionSetValueCollection"
            };

            var actual = EntityConverterHelper.GetAttributeValueForCsv(attribute);

            actual.Should().Be("12|3");
        }

        [TestMethod]
        public void GetAttributeValueForCsvMoney()
        {
            var input = new Money(new decimal(12.00));
            var attribute = new CrmAttributeStore(new KeyValuePair<string, object>("firstname", input))
            {
                AttributeType = "Microsoft.Xrm.Sdk.Money"
            };

            var actual = EntityConverterHelper.GetAttributeValueForCsv(attribute);

            actual.Should().Be(input.Value);
        }

        [TestMethod]
        public void GetAttributeValueForCsvByte()
        {
            byte[] input = new byte[5];
            var attribute = new CrmAttributeStore(new KeyValuePair<string, object>("firstname", input))
            {
                AttributeType = "System.Byte[]"
            };

            FluentActions.Invoking(() => EntityConverterHelper.GetAttributeValueForCsv(attribute))
                         .Should()
                         .Throw<ConfigurationException>();
        }

        [TestMethod]
        public void GetAttributeValueForCsvEntityCollection()
        {
            var list = new List<Entity>
            {
                new Entity("account", Guid.NewGuid())
            };
            EntityCollection input = new EntityCollection(list);
            var attribute = new CrmAttributeStore(new KeyValuePair<string, object>("firstname", input))
            {
                AttributeType = "Microsoft.Xrm.Sdk.EntityCollection"
            };

            FluentActions.Invoking(() => EntityConverterHelper.GetAttributeValueForCsv(attribute))
                         .Should()
                         .Throw<ConfigurationException>();
        }

        [TestMethod]
        public void GetAttributeValueForCsvAliasedValue()
        {
            var input = new AliasedValue("contact", "contactid", Guid.NewGuid().ToString());
            var attribute = new CrmAttributeStore(new KeyValuePair<string, object>("firstname", input))
            {
                AttributeType = "Microsoft.Xrm.Sdk.AliasedValue"
            };

            var actual = EntityConverterHelper.GetAttributeValueForCsv(attribute);

            actual.Should().Be(input.Value);
        }
    }
}