using System;
using System.Text;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Extensions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Capgemini.Xrm.DataMigration.Extensions
{
    public static class ExecuteMultipleResponseItemExtensions
    {
        public static OperationType GetOperationType(this ExecuteMultipleResponseItem response)
        {
            response.ThrowArgumentNullExceptionIfNull(nameof(response));

            if (response.Fault != null)
            {
                return OperationType.Failed;
            }

            switch (response.Response.GetType().Name)
            {
                case nameof(AssignResponse):
                    return OperationType.Assign;

                case nameof(AssociateResponse):
                    return OperationType.Associate;

                case nameof(CreateResponse):
                    return OperationType.Create;

                case nameof(UpdateResponse):
                    return OperationType.Update;

                case nameof(UpsertResponse):
                    var responseTyped = response.Response as UpsertResponse;
                    return responseTyped.RecordCreated ? OperationType.Create : OperationType.Update;

                default:
                    throw new ArgumentOutOfRangeException($"Type {response.Response.GetType().Name} is not supported");
            }
        }

        public static string GetOperationMessage(this ExecuteMultipleResponseItem response, Entity entity)
        {
            entity.ThrowArgumentNullExceptionIfNull(nameof(entity));
            response.ThrowArgumentNullExceptionIfNull(nameof(response));

            if (response.Fault != null)
            {
                return GetOperationResultError(response.Fault);
            }

            switch (response.Response.GetType().Name)
            {
                case nameof(CreateResponse):
                    var createResponse = response.Response as CreateResponse;
                    return $"Entity {entity.LogicalName}:{createResponse.id} {response.Response.ResponseName}";

                case nameof(UpsertResponse):
                    var upsertResponse = response.Response as UpsertResponse;
                    return $"Entity {entity.LogicalName}:{upsertResponse.Target.Id} {response.Response.ResponseName}";

                default:
                    return $"Entity {entity.LogicalName}:{entity.Id} {response.Response.ResponseName}";
            }
        }

        private static string GetOperationResultError(OrganizationServiceFault fault)
        {
            StringBuilder error = new StringBuilder();

            if (fault != null)
            {
                if (fault.InnerFault != null)
                {
                    error.Append(GetOperationResultError(fault.InnerFault));
                }
                else
                {
                    error.Append($"Error:{fault.Message}");
                }
            }

            return error.ToString();
        }
    }
}