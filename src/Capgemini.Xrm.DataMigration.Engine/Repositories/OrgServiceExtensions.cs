using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capgemini.Xrm.DataMigration.Repositories
{
    public static class OrgServiceExtensions
    {
        public static EntityCollection GetEntitiesByColumn(this IOrganizationService orgService, string entityName, string columnName, object columnValue, string[] columnsToRetrieve = null, int pageSize = 100)
        {

            var query = new QueryExpression(entityName);
            query.ColumnSet = columnsToRetrieve != null ? new ColumnSet(columnsToRetrieve) : new ColumnSet(true);

            if (!string.IsNullOrWhiteSpace(columnName) && columnValue != null)
                query.Criteria.AddCondition(columnName, ConditionOperator.Equal, columnValue);
            else if (!string.IsNullOrWhiteSpace(columnName) && columnValue == null)
                query.Criteria.AddCondition(columnName, ConditionOperator.Null);

            return orgService.GetDataByQuery(query, pageSize);
        }

        public static EntityCollection GetDataByQuery(this IOrganizationService orgService, QueryExpression query, int pageSize)
        {
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

                if (query.PageInfo.PageNumber == 1)
                    allResults = pagedResults;
                else
                    allResults.Entities.AddRange(pagedResults.Entities);


                if (pagedResults.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = pagedResults.PagingCookie;
                }
                else
                    break;
            }

            return allResults;
        }
    }
}
