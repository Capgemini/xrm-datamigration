using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.DataMigration.Exceptions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Moq;

namespace Capgemini.Xrm.DataMigration.Cache.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class EntityMetadataCacheTests : UnitTestBase
    {
        private EntityMetadataCache systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();

            systemUnderTest = new EntityMetadataCache(MockOrganizationService.Object);
        }

        [TestMethod]
        public void EntityMetadataCache()
        {
            FluentActions.Invoking(() => new EntityMetadataCache(MockOrganizationService.Object))
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void GetEntityMetadata()
        {
            string entityName = "contactemd";

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse());

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);

            FluentActions.Invoking(() => systemUnderTest.GetEntityMetadata(entityName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
        }

        [TestMethod]
        public void GetLookUpEntityNameNoEntityFound()
        {
            string entityName = "contactluenNoEntityFound";
            string attributeName = "contactluenid";

            var manyToManyRelationships = new List<OneToManyRelationshipMetadata>();

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), null, manyToManyRelationships);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);

            FluentActions.Invoking(() => systemUnderTest.GetLookUpEntityName(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
        }

        [TestMethod]
        public void GetLookUpEntityName()
        {
            string entityName = "contactluen";
            string attributeName = "contactluenid";

            var oneToManyRelationshipMetadata = new OneToManyRelationshipMetadata
            {
                ReferencingAttribute = attributeName
            };
            var manyToManyRelationships = new List<OneToManyRelationshipMetadata>
            {
                oneToManyRelationshipMetadata
            };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), null, manyToManyRelationships);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);

            FluentActions.Invoking(() => systemUnderTest.GetLookUpEntityName(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
        }

        [TestMethod]
        public void GetManyToManyEntityDetails()
        {
            string intersectEntityName = "accountcontactmtmed";

            var relationship = new ManyToManyRelationshipMetadata
            {
                IntersectEntityName = intersectEntityName
            };

            var manyToManyRelationships = new List<ManyToManyRelationshipMetadata>
            {
                relationship
            };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), null, null, manyToManyRelationships, true);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);

            FluentActions.Invoking(() => systemUnderTest.GetManyToManyEntityDetails(intersectEntityName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
        }

        [TestMethod]
        public void GetAttributeFindsNoEntity()
        {
            string entityName = "contactattnoentity";
            string attributeName = "contactattid";

            AttributeMetadata attributeMetaDataItem = new AttributeMetadata
            {
                LogicalName = "contactattnoentity1"
            };

            var attributes = new List<AttributeMetadata>
            {
                attributeMetaDataItem
            };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);

            AttributeMetadata actual = null;
            FluentActions.Invoking(() => actual = systemUnderTest.GetAttribute(entityName, attributeName))
                        .Should()
                        .Throw<ConfigurationException>();

            MockOrganizationService.VerifyAll();
        }

        [TestMethod]
        public void GetAttributeFindsNoAttribute()
        {
            string entityName = "contactattnoattributes";
            string attributeName = "contactattidnoattribute";

            AttributeMetadata attributeMetaDataItem = new AttributeMetadata
            {
                LogicalName = "contactattnoattributes1"
            };

            var attributes = new List<AttributeMetadata>
            {
                attributeMetaDataItem
            };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);

            AttributeMetadata actual = null;
            FluentActions.Invoking(() => actual = systemUnderTest.GetAttribute(entityName, attributeName))
                        .Should()
                        .Throw<ConfigurationException>();

            MockOrganizationService.VerifyAll();
        }

        [TestMethod]
        public void GetAttribute()
        {
            string entityName = "contactatt";
            string attributeName = "contactattid";

            AttributeMetadata attributeMetaDataItem = new AttributeMetadata
            {
                LogicalName = attributeName
            };

            var attributes = new List<AttributeMetadata>
            {
                attributeMetaDataItem
            };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);

            AttributeMetadata actual = null;
            FluentActions.Invoking(() => actual = systemUnderTest.GetAttribute(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();

            actual.LogicalName.Should().Be(attributeName);
        }

        [TestMethod]
        public void GetAttributeDotNetTypeBool()
        {
            string entityName = "contactadntbool";
            string attributeName = "contactadntbool";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.Boolean);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);

            Type actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(bool), actual);
        }

        [TestMethod]
        public void GetAttributeDotNetTypeDateTime()
        {
            string entityName = "contactadntdatetime";
            string attributeName = "contactadntDateTime";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.DateTime);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);

            Type actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(DateTime), actual);
        }

        [TestMethod]
        public void GetAttributeDotNetTypeDecimal()
        {
            string entityName = "contactadntdecimal";
            string attributeName = "contactadntdecimal";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.Decimal);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);

            Type actual = null;
            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(decimal), actual);
        }

        [TestMethod]
        public void GetAttributeDotNetTypeDouble()
        {
            string entityName = "contactadntdouble";
            string attributeName = "contactadntdouble";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.Double);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);

            Type actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(double), actual);
        }

        [TestMethod]
        public void GetAttributeDotNetTypeInt()
        {
            string entityName = "contactadntint";
            string attributeName = "contactadntint";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.Integer);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);

            Type actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(int), actual);
        }

        [TestMethod]
        public void GetAttributeDotNetTypeLookUp()
        {
            string entityName = "contactadntlookup";
            string attributeName = "contactadntlkup";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.Lookup);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);

            Type actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(EntityReference), actual);
        }

        [TestMethod]
        public void GetAttributeDotNetTypeOwner()
        {
            string entityName = "contactadntowner";
            string attributeName = "contactadntowner";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.Owner);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);

            Type actual = null;
            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(EntityReference), actual);
        }

        [TestMethod]
        public void GetAttributeDotNetTypeCustomer()
        {
            string entityName = "contactadntcustomer";
            string attributeName = "contactadntcustomer";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.Customer);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);

            Type actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(EntityReference), actual);
        }

        [TestMethod]
        public void GetAttributeDotNetTypeMoney()
        {
            string entityName = "contactadntmoney";
            string attributeName = "contactadntmoney";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.Money);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);
            Type actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(Money), actual);
        }

        [TestMethod]
        public void GetAttributeDotNetTypePartyList()
        {
            string entityName = "contactadntPartyList";
            string attributeName = "contactadntPartyList";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.PartyList);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);
            Type actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(EntityCollection), actual);
        }

        [TestMethod]
        public void GetAttributeDotNetTypeCalendarRules()
        {
            string entityName = "contactadntCalendarRules";
            string attributeName = "contactadntCalendarRules";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.CalendarRules);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);
            Type actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(EntityCollection), actual);
        }

        [TestMethod]
        public void GetAttributeDotNetTypePicklist()
        {
            string entityName = "contactadntPicklist";
            string attributeName = "contactadntPicklist";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.Picklist);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);
            Type actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(OptionSetValue), actual);
        }

        [TestMethod]
        public void GetAttributeDotNetTypeState()
        {
            string entityName = "contactadntState";
            string attributeName = "contactadntState";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.State);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);
            Type actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(OptionSetValue), actual);
        }

        [TestMethod]
        public void GetAttributeDotNetTypeStatus()
        {
            string entityName = "contactadntStatus";
            string attributeName = "contactadntStatus";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.Status);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);
            Type actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(OptionSetValue), actual);
        }

        [TestMethod]
        public void GetAttributeDotNetTypeString()
        {
            string entityName = "contactadntString";
            string attributeName = "contactadntString";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.String);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);
            Type actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(string), actual);
        }

        [TestMethod]
        public void GetAttributeDotNetTypeEntityName()
        {
            string entityName = "contactadntEntityName";
            string attributeName = "contactadntEntityName";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.EntityName);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);
            Type actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(string), actual);
        }

        [TestMethod]
        public void GetAttributeDotNetTypeMemo()
        {
            string entityName = "contactadntMemo";
            string attributeName = "contactadntMemo";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.Memo);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);
            Type actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(string), actual);
        }

        [TestMethod]
        public void GetAttributeDotNetTypeBigInt()
        {
            string entityName = "contactadntBigInt";
            string attributeName = "contactadntBigInt";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.BigInt);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);
            Type actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(long), actual);
        }

        [TestMethod]
        public void GetAttributeDotNetTypeManagedProperty()
        {
            string entityName = "contactadntManagedProperty";
            string attributeName = "contactadntManagedProperty";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.ManagedProperty);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);
            Type actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                         .Throw<ValidationException>();

            MockOrganizationService.VerifyAll();
        }

        [TestMethod]
        public void GetAttributeDotNetTypeVirtual()
        {
            string entityName = "contactadntVirtual";
            string attributeName = "contactadntVirtual";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.Virtual);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);
            Type actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .Throw<ValidationException>();

            MockOrganizationService.VerifyAll();
        }

        [TestMethod]
        public void GetAttributeDotNetTypeUniqueidentifier()
        {
            string entityName = "contactadntguid1";
            string attributeName = "contactadntid";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.Uniqueidentifier);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            RetrieveEntityResponse response = InjectMetaDataToResponse(new RetrieveEntityResponse(), attributes);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);

            Type actual = null;
            FluentActions.Invoking(() => actual = systemUnderTest.GetAttributeDotNetType(entityName, attributeName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            Assert.AreEqual(typeof(Guid), actual);
        }

        [TestMethod]
        public void GetIdAliasKey()
        {
            string entityName = "contactIdAliasKey";
            string attributeName = "contactIdAliasKeyid";

            var attributeMetaDataItem = new AttributeMetadata { LogicalName = attributeName };

            SetFieldValue(attributeMetaDataItem, "_attributeType", AttributeTypeCode.Uniqueidentifier);

            var attributes = new List<AttributeMetadata> { attributeMetaDataItem };

            var oneToManyRelationshipMetadata = new OneToManyRelationshipMetadata
            {
                ReferencingAttribute = attributeName
            };
            var oneToManyRelationships = new List<OneToManyRelationshipMetadata>
            {
                oneToManyRelationshipMetadata
            };

            var entityMetaData = InitializeEntityMetadata(attributes, oneToManyRelationships);
            SetFieldValue(entityMetaData, "_primaryIdAttribute", attributeName);

            var response = InjectMetaDataToResponse(new RetrieveEntityResponse(), entityMetaData);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);

            string actual = null;

            FluentActions.Invoking(() => actual = systemUnderTest.GetIdAliasKey(entityName))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
            actual.Should().NotBeNullOrEmpty();
        }
    }
}