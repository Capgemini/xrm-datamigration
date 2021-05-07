using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.DataMigration.Exceptions;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Model;
using FluentAssertions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Moq;

namespace Capgemini.Xrm.DataMigration.Repositories.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public abstract class EntityRepositoryTests : UnitTestBase
    {
        private readonly string entityName = "contact";
        private readonly string[] filterFields = new string[] { "firstname", "lastname", "dateOfBirth" };
        private readonly object[] filterValues = new string[] { "joe", "blogs", "20 June 2019" };
        private readonly RepositoryCachingMode cachingMode;
        private TestOrganizationalService testOrgService;
        private List<EntityWrapper> entities = new List<EntityWrapper>();
        private ExecuteMultipleResponse requestResult;
        private EntityRepository systemUnderTest;

        public EntityRepositoryTests(RepositoryCachingMode cachingMode)
        {
            this.cachingMode = cachingMode;
        }

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();

            var contact = new Entity(entityName);
            contact.Attributes.Add("contactid", Guid.NewGuid());
            var account = new Entity("account");
            account.Attributes.Add("accountid", Guid.NewGuid());

            entities = new List<EntityWrapper>
            {
                new EntityWrapper(contact),
                new EntityWrapper(account)
            };
            requestResult = new ExecuteMultipleResponse();
            testOrgService = new TestOrganizationalService();

            testOrgService.EntityCollection.Entities.AddRange(new Entity[] { contact, account });

            MockEntityMetadataCache = new Mock<IEntityMetadataCache>();

            systemUnderTest = new EntityRepository(testOrgService, MockRetryExecutor.Object, MockEntityMetadataCache.Object, cachingMode);
        }

        [TestMethod]
        public void EntityRepository()
        {
            FluentActions.Invoking(() => new EntityRepository(testOrgService, MockRetryExecutor.Object, RepositoryCachingMode.Lookup))
                 .Should()
                 .NotThrow();
        }

        [TestMethod]
        public void EntityRepositorySecondConstructor()
        {
            FluentActions.Invoking(() => new EntityRepository(testOrgService, MockRetryExecutor.Object, MockEntityMetadataCache.Object, cachingMode))
                 .Should()
                 .NotThrow();
        }

        [TestMethod]
        public void CreateUpdateEntities()
        {
            testOrgService.ExecutionResponse = new ExecuteMultipleResponse();

            MockRetryExecutor.Setup(a => a.Execute(It.IsAny<Func<ExecuteMultipleResponse>>()))
                         .Returns(requestResult);

            FluentActions.Invoking(() => systemUnderTest.CreateUpdateEntities(entities))
                .Should()
                .NotThrow();
        }

        [TestMethod]
        public void CreateEntities()
        {
            testOrgService.ExecutionResponse = new ExecuteMultipleResponse();

            MockRetryExecutor.Setup(a => a.Execute(It.IsAny<Func<ExecuteMultipleResponse>>()))
                         .Returns(requestResult);

            FluentActions.Invoking(() => systemUnderTest.CreateEntities(entities))
                        .Should()
                        .NotThrow();
        }

        [TestMethod]
        public void UpdateEntities()
        {
            testOrgService.ExecutionResponse = new ExecuteMultipleResponse();

            var testRetryService = new TestRetryExecutor();
            systemUnderTest = new EntityRepository(testOrgService, testRetryService, MockEntityMetadataCache.Object, cachingMode);

            FluentActions.Invoking(() => systemUnderTest.UpdateEntities(entities))
                        .Should()
                        .NotThrow();
        }

        [TestMethod]
        public void AssignEntities()
        {
            testOrgService.ExecutionResponse = new ExecuteMultipleResponse();

            MockRetryExecutor.Setup(a => a.Execute(It.IsAny<Func<ExecuteMultipleResponse>>()))
                         .Returns(requestResult);

            FluentActions.Invoking(() => systemUnderTest.AssignEntities(entities))
                        .Should()
                        .NotThrow();

            MockRetryExecutor.VerifyAll();
        }

        [TestMethod]
        public void AssociateManyToManyEntity()
        {
            testOrgService.ExecutionResponse = new RetrieveEntityResponse();

            MockRetryExecutor.Setup(a => a.Execute(It.IsAny<Func<ExecuteMultipleResponse>>()))
                         .Returns(requestResult);
            ManyToManyDetails details = new ManyToManyDetails
            {
                Entity1IntersectAttribute = "contactid",
                Entity2IntersectAttribute = "accountid",
                IsManyToMany = true,
                SchemaName = "accountcontact"
            };

            MockEntityMetadataCache.Setup(a => a.GetManyToManyEntityDetails(It.IsAny<string>()))
                                .Returns(details);

            FluentActions.Invoking(() => systemUnderTest.AssociateManyToManyEntity(entities))
                        .Should()
                        .NotThrow();

            MockRetryExecutor.VerifyAll();
            MockEntityMetadataCache.VerifyAll();
        }

        [TestMethod]
        public void GetParentBuId()
        {
            FluentActions.Invoking(() => systemUnderTest.GetParentBuId())
                        .Should()
                        .NotThrow();
        }

        [TestMethod]
        public void GetAllEntitesMetadata()
        {
            systemUnderTest = new EntityRepository(MockOrganizationService.Object, MockRetryExecutor.Object, MockEntityMetadataCache.Object, cachingMode);

            RetrieveAllEntitiesResponse response = new RetrieveAllEntitiesResponse();

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>()))
                           .Returns(response);

            FluentActions.Invoking(() => systemUnderTest.GetAllEntitesMetadata())
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
        }

        [TestMethod]
        public void GetEntitiesByName()
        {
            string[] collumns = new string[] { "firstname", "lastname", "dateOfBirth" };
            int pageSize = 10;

            FluentActions.Invoking(() => systemUnderTest.GetEntitiesByName(entityName, collumns, pageSize))
                        .Should()
                        .NotThrow();
        }

        [TestMethod]
        public void DeleteEntity()
        {
            Guid entityId = Guid.NewGuid();

            systemUnderTest = new EntityRepository(MockOrganizationService.Object, MockRetryExecutor.Object, MockEntityMetadataCache.Object, cachingMode);

            MockOrganizationService.Setup(a => a.Delete(It.IsAny<string>(), It.IsAny<Guid>()));

            FluentActions.Invoking(() => systemUnderTest.DeleteEntity(entityName, entityId))
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
        }

        [TestMethod]
        public void GetOrganizationId()
        {
            WhoAmIResponse response = new WhoAmIResponse();

            systemUnderTest = new EntityRepository(MockOrganizationService.Object, MockRetryExecutor.Object, MockEntityMetadataCache.Object, cachingMode);

            MockOrganizationService.Setup(a => a.Execute(It.IsAny<OrganizationRequest>())).Returns(response);

            FluentActions.Invoking(() => systemUnderTest.GetOrganizationId())
                        .Should()
                        .NotThrow();

            MockOrganizationService.VerifyAll();
        }

        [TestMethod]
        public void FindEntitiesByName()
        {
            string nameValue = "Joe";
            var entityMetaData = new EntityMetadata();

            MockEntityMetadataCache.Setup(a => a.GetEntityMetadata(It.IsAny<string>())).Returns(entityMetaData);

            FluentActions.Invoking(() => systemUnderTest.FindEntitiesByName(entityName, nameValue))
                        .Should()
                        .NotThrow();
        }

        [TestMethod]
        public void GetGuidForMappingThrowsExceptionForNullFilterValues()
        {
            var errorMessage = "Filter fields must have same length as filter values!";

            FluentActions.Invoking(() => systemUnderTest.GetGuidForMapping(entityName, filterFields, null))
                         .Should()
                         .Throw<ArgumentException>()
                         .Where(a => a.Message.Contains(errorMessage));
        }

        [TestMethod]
        public void GetGuidForMappingThrowsExceptionForNullFilterFields()
        {
            var errorMessage = "Filter fields must have same length as filter values!";

            FluentActions.Invoking(() => systemUnderTest.GetGuidForMapping(entityName, null, filterValues))
                         .Should()
                         .Throw<ArgumentException>()
                         .Where(a => a.Message.Contains(errorMessage));
        }

        [TestMethod]
        public void GetGuidForMappingThrowsExceptionForFilterFieldsNotEqualToFilterValues()
        {
            string[] localFilterFields = new string[] { "firstname", "lastname" };

            var errorMessage = "Filter fields must have same length as filter values!";

            FluentActions.Invoking(() => systemUnderTest.GetGuidForMapping(entityName, localFilterFields, filterValues))
                         .Should()
                         .Throw<ArgumentException>()
                         .Where(a => a.Message.Contains(errorMessage));
        }

        [TestMethod]
        public void GetGuidForMapping()
        {
            testOrgService.EntityCollection.Entities.Clear();
            var contact = new Entity("contact");
            contact.Attributes.Add("contactid", Guid.NewGuid());
            testOrgService.EntityCollection.Entities.AddRange(new Entity[] { contact });

            FluentActions.Invoking(() => systemUnderTest.GetGuidForMapping(entityName, filterFields, filterValues))
                        .Should()
                        .NotThrow();
        }

        [TestMethod]
        public void GetGuidForMappingFailsWhenMoreThanOneEntityIsReturnedFromCrm()
        {
            var errorMessage = $"incorrect mapping value - cannot find unique record, Found {entities.Count} maching criteria {entityName}:{string.Join(",", filterFields)}={string.Join(", ", filterValues)}";

            FluentActions.Invoking(() => systemUnderTest.GetGuidForMapping(entityName, filterFields, filterValues))
                        .Should()
                        .Throw<ConfigurationException>()
                        .Where(a => a.Message.Contains(errorMessage));
        }

        [TestMethod]
        public void GetTotalRecordCount()
        {
            var actual = -1;

            FluentActions.Invoking(() => actual = systemUnderTest.GetTotalRecordCount(entityName, filterFields, filterValues))
                        .Should()
                        .NotThrow();

            actual.Should().NotBe(-1);
        }
    }
}