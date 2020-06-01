using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core.Tests.Base;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Capgemini.Xrm.DataMigration.Extensions.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class OrgServiceExtensionsTests
    {
        private TestOrganizationalService testOrgService;

        [TestInitialize]
        public void Setup()
        {
            testOrgService = new TestOrganizationalService();

            testOrgService.EntityCollection.Entities.Add(new Entity("contact") { });
        }

        [TestMethod]
        public void GetEntitiesByColumn()
        {
            string entityName = "contact";
            string columnName = "firstname";
            string columnValue = "Joe";
            string[] columnsToRetrieve = new string[] { "firstname", "lastname", "dateOfBirth" };

            EntityCollection actual = null;

            FluentActions.Invoking(() => actual = testOrgService.GetEntitiesByColumn(entityName, columnName, columnValue, columnsToRetrieve))
                .Should()
                .NotThrow();

            Assert.AreEqual(testOrgService.EntityCollection.Entities.Count, actual.Entities.Count);
            Assert.IsTrue(testOrgService.QueryExpression.Criteria.Conditions.Count == 1);

            var condition = testOrgService.QueryExpression.Criteria.Conditions[0];

            Assert.IsTrue(condition.AttributeName == columnName);
            Assert.IsTrue(condition.Operator == ConditionOperator.Equal);
            Assert.IsTrue(condition.Values[0].ToString() == columnValue);
        }

        [TestMethod]
        public void GetEntitiesByColumnNullColumnValue()
        {
            string entityName = "contact";
            string columnName = "firstname";
            string columnValue = null;
            string[] columnsToRetrieve = new string[] { "firstname", "lastname", "dateOfBirth" };

            EntityCollection actual = null;

            FluentActions.Invoking(() => actual = testOrgService.GetEntitiesByColumn(entityName, columnName, columnValue, columnsToRetrieve))
                .Should()
                .NotThrow();

            Assert.AreEqual(testOrgService.EntityCollection.Entities.Count, actual.Entities.Count);
            Assert.IsTrue(testOrgService.QueryExpression.Criteria.Conditions.Count == 1);

            var condition = testOrgService.QueryExpression.Criteria.Conditions[0];

            Assert.IsTrue(condition.AttributeName == columnName);
            Assert.IsTrue(condition.Operator == ConditionOperator.Null);
            Assert.IsTrue(condition.Values.Count == 0);
        }

        [TestMethod]
        public void GetEntitiesByColumnNullColumnName()
        {
            string entityName = "contact";
            string columnName = null;
            string columnValue = null;
            string[] columnsToRetrieve = new string[] { "firstname", "lastname", "dateOfBirth" };

            EntityCollection actual = null;

            FluentActions.Invoking(() => actual = testOrgService.GetEntitiesByColumn(entityName, columnName, columnValue, columnsToRetrieve))
                .Should()
                .NotThrow();

            Assert.AreEqual(testOrgService.EntityCollection.Entities.Count, actual.Entities.Count);
            Assert.IsTrue(testOrgService.QueryExpression.Criteria.Conditions.Count == 0);
        }

        [TestMethod]
        public void GetDataByQuery()
        {
            QueryExpression query = new QueryExpression();
            int pageSize = 10;
            bool shouldIncudeEntityCollection = true;

            EntityCollection actual = null;

            FluentActions.Invoking(() => actual = testOrgService.GetDataByQuery(query, pageSize, shouldIncudeEntityCollection))
                .Should()
                .NotThrow();

            Assert.AreEqual(testOrgService.EntityCollection.Entities.Count, actual.Entities.Count);
        }

        [TestMethod]
        public void ExecuteMultiple()
        {
            testOrgService.ExecutionResponse = new ExecuteMultipleResponse();
            OrganizationRequestCollection orgRequests = new OrganizationRequestCollection();

            ExecuteMultipleResponse actual = null;

            FluentActions.Invoking(() => actual = testOrgService.ExecuteMultiple(orgRequests))
                .Should()
                .NotThrow();
        }
    }
}