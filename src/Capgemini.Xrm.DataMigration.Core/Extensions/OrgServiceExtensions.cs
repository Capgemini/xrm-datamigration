using Capgemini.DataMigration.Core.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Capgemini.Xrm.DataMigration.Extensions
{
    public static class OrgServiceExtensions
    {
        public static EntityCollection GetEntitiesByColumn(this IOrganizationService orgService, string entityName, string columnName, object columnValue, string[] columnsToRetrieve = null, int pageSize = 100)
        {
            var query = new QueryExpression(entityName)
            {
                ColumnSet = columnsToRetrieve != null ? new ColumnSet(columnsToRetrieve) : new ColumnSet(true)
            };

            if (!string.IsNullOrWhiteSpace(columnName))
            {
                if (columnValue != null)
                {
                    query.Criteria.AddCondition(columnName, ConditionOperator.Equal, columnValue);
                }
                else
                {
                    query.Criteria.AddCondition(columnName, ConditionOperator.Null);
                }
            }

            return orgService.GetDataByQuery(query, pageSize, true);
        }

        public static EntityCollection GetDataByQuery(this IOrganizationService orgService, QueryExpression query, int pageSize, bool shouldIncudeEntityCollection = true, int maxRecords = int.MaxValue)
        {
            query.ThrowArgumentNullExceptionIfNull(nameof(query));
            orgService.ThrowArgumentNullExceptionIfNull(nameof(orgService));

            var allResults = new EntityCollection();

            query.PageInfo = new PagingInfo
            {
                Count = pageSize,
                PageNumber = 1,
                PagingCookie = null
            };

            while (true)
            {
                var pagedResults = orgService.RetrieveMultiple(query);

                if (shouldIncudeEntityCollection)
                {
                    if (query.PageInfo.PageNumber == 1)
                    {
                        allResults = pagedResults;
                    }
                    else
                    {
                        allResults.Entities.AddRange(pagedResults.Entities);
                    }
                }
                else
                {
                    // Update the count of pages retrieved and processed
                    allResults.TotalRecordCount += pagedResults.Entities.Count;
                }

                maxRecords -= pagedResults.Entities.Count;

                if (pagedResults.MoreRecords && maxRecords > 0)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = pagedResults.PagingCookie;
                }
                else
                {
                    break;
                }
            }

            return allResults;
        }

        public static ExecuteMultipleResponse ExecuteMultiple(this IOrganizationService orgService, OrganizationRequestCollection orgRequests)
        {
            orgService.ThrowArgumentNullExceptionIfNull(nameof(orgService));

            var requestWithResults = new ExecuteMultipleRequest
            {
                Settings = new ExecuteMultipleSettings
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            requestWithResults.Requests = orgRequests;

            var responseWithResults = (ExecuteMultipleResponse)orgService.Execute(requestWithResults);

            return responseWithResults;
        }
    }
}