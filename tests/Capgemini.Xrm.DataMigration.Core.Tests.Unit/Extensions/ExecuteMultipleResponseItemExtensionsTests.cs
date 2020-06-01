using System;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core;
using FluentAssertions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Capgemini.Xrm.DataMigration.Extensions.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ExecuteMultipleResponseItemExtensionsTests
    {
        [TestMethod]
        public void GetOperationTypeNullExecuteMultipleResponseItem()
        {
            ExecuteMultipleResponseItem response = null;
            OperationType actual = OperationType.Update;

            FluentActions.Invoking(() => actual = response.GetOperationType())
                         .Should()
                         .Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void GetOperationTypeWithFault()
        {
            var response = new ExecuteMultipleResponseItem
            {
                Fault = new OrganizationServiceFault()
            };
            OperationType actual = OperationType.Update;

            FluentActions.Invoking(() => actual = response.GetOperationType())
                         .Should()
                         .NotThrow();

            actual.Should().Be(OperationType.Failed);
        }

        [TestMethod]
        public void GetOperationTypeWithExecuteMultipleResponseItemHavingNullResponse()
        {
            var response = new ExecuteMultipleResponseItem();
            OperationType actual = OperationType.Update;

            FluentActions.Invoking(() => actual = response.GetOperationType())
                         .Should()
                         .Throw<NullReferenceException>();
        }

        [TestMethod]
        public void GetOperationTypeWithExecuteMultipleResponseItemHavingOrganizationResponse()
        {
            var response = new ExecuteMultipleResponseItem
            {
                Response = new OrganizationResponse()
            };
            OperationType actual = OperationType.Update;

            FluentActions.Invoking(() => actual = response.GetOperationType())
                         .Should()
                         .Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void GetOperationTypeWithExecuteMultipleResponseItemHavingAssignResponse()
        {
            var response = new ExecuteMultipleResponseItem
            {
                Response = new AssignResponse()
            };
            OperationType actual = OperationType.Update;

            FluentActions.Invoking(() => actual = response.GetOperationType())
                         .Should()
                         .NotThrow();

            actual.Should().Be(OperationType.Assign);
        }

        [TestMethod]
        public void GetOperationTypeWithExecuteMultipleResponseItemHavingAssociateResponse()
        {
            var response = new ExecuteMultipleResponseItem
            {
                Response = new AssociateResponse()
            };
            OperationType actual = OperationType.Update;

            FluentActions.Invoking(() => actual = response.GetOperationType())
                         .Should()
                         .NotThrow();

            actual.Should().Be(OperationType.Associate);
        }

        [TestMethod]
        public void GetOperationTypeWithExecuteMultipleResponseItemHavingCreateResponse()
        {
            var response = new ExecuteMultipleResponseItem
            {
                Response = new CreateResponse()
            };
            OperationType actual = OperationType.Update;

            FluentActions.Invoking(() => actual = response.GetOperationType())
                         .Should()
                         .NotThrow();

            actual.Should().Be(OperationType.Create);
        }

        [TestMethod]
        public void GetOperationTypeWithExecuteMultipleResponseItemHavingUpdateResponse()
        {
            var response = new ExecuteMultipleResponseItem
            {
                Response = new UpdateResponse()
            };
            OperationType actual = OperationType.Update;

            FluentActions.Invoking(() => actual = response.GetOperationType())
                         .Should()
                         .NotThrow();

            actual.Should().Be(OperationType.Update);
        }

        [TestMethod]
        public void GetOperationTypeWithExecuteMultipleResponseItemHavingUpsertResponse()
        {
            var response = new UpsertResponse();
            var responseItem = new ExecuteMultipleResponseItem
            {
                Response = response
            };
            OperationType actual = OperationType.Update;

            FluentActions.Invoking(() => actual = responseItem.GetOperationType())
                         .Should()
                         .NotThrow();

            actual.Should().Be(OperationType.Update);
        }

        [TestMethod]
        public void GetOperationMessageNullEntity()
        {
            var responseItem = new ExecuteMultipleResponseItem
            {
            };
            string actual = null;

            FluentActions.Invoking(() => actual = responseItem.GetOperationMessage(null))
                         .Should()
                         .Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void GetOperationMessageNullExecuteMultipleResponseItem()
        {
            Entity entity = new Entity("contact", Guid.NewGuid());
            ExecuteMultipleResponseItem responseItem = null;
            string actual = null;

            FluentActions.Invoking(() => actual = responseItem.GetOperationMessage(entity))
                         .Should()
                         .Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void GetOperationMessageWithExecuteMultipleResponseItemHavingFault()
        {
            Entity entity = new Entity("contact", Guid.NewGuid());
            var responseItem = new ExecuteMultipleResponseItem
            {
                Fault = new OrganizationServiceFault()
            };
            string actual = null;

            FluentActions.Invoking(() => actual = responseItem.GetOperationMessage(entity))
                         .Should()
                         .NotThrow();

            actual.Should().Contain("Error:");
        }

        [TestMethod]
        public void GetOperationMessageWithExecuteMultipleResponseItemHavingInnerFault()
        {
            var testMessage = "Random Test message!";
            Entity entity = new Entity("contact", Guid.NewGuid());
            var responseItem = new ExecuteMultipleResponseItem
            {
                Fault = new OrganizationServiceFault()
                {
                    InnerFault = new OrganizationServiceFault() { Message = testMessage }
                }
            };
            string actual = null;

            FluentActions.Invoking(() => actual = responseItem.GetOperationMessage(entity))
                         .Should()
                         .NotThrow();

            actual.Should().Contain(testMessage);
        }

        [TestMethod]
        public void GetOperationMessageWithExecuteMultipleResponseItemHavingNoresponse()
        {
            Entity entity = new Entity("contact", Guid.NewGuid());
            var responseItem = new ExecuteMultipleResponseItem
            {
            };
            string actual = null;

            FluentActions.Invoking(() => actual = responseItem.GetOperationMessage(entity))
                         .Should()
                         .Throw<NullReferenceException>();
        }

        [TestMethod]
        public void GetOperationMessage()
        {
            var entityName = "contact";
            var id = Guid.NewGuid();
            Entity entity = new Entity(entityName, id);
            var responseItem = new ExecuteMultipleResponseItem
            {
                Response = new OrganizationResponse()
            };
            var expected = $"Entity {entityName}:{id} {responseItem.Response.ResponseName}";
            string actual = null;

            FluentActions.Invoking(() => actual = responseItem.GetOperationMessage(entity))
                         .Should()
                         .NotThrow();

            actual.Should().Be(expected);
        }

        [TestMethod]
        public void GetOperationMessageCreateResponse()
        {
            var entityName = "contact";
            var id = Guid.Empty;
            Entity entity = new Entity(entityName, id);
            var responseItem = new ExecuteMultipleResponseItem
            {
                Response = new CreateResponse()
            };
            var expected = $"Entity {entityName}:{id} {responseItem.Response.ResponseName}";
            string actual = null;

            FluentActions.Invoking(() => actual = responseItem.GetOperationMessage(entity))
                         .Should()
                         .NotThrow();

            actual.Should().Be(expected);
        }
    }
}