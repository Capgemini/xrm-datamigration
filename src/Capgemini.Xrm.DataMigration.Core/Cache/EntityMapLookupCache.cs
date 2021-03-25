using System;
using System.Linq;
using Capgemini.DataMigration.Cache;
using Capgemini.DataMigration.Exceptions;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.Extensions;
using Capgemini.Xrm.DataMigration.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Capgemini.Xrm.DataMigration.Cache
{
    public class EntityMapLookupCache : SimpleMemoryCache<Guid?>
    {
        private const string CacheId = "CRMMapCache";
        private readonly IOrganizationService orgService;

        public EntityMapLookupCache(IOrganizationService orgService)
        {
            this.orgService = orgService;
        }

        public Guid GetGuidForMapping(string entityName, string[] filterFields, object[] filterValues)
        {
            if (filterFields == null || filterValues == null || filterFields.Length != filterValues.Length)
            {
                throw new ArgumentException("filter fields must have same length as filter values!");
            }

            var cacheKey = CacheId + "|" + entityName + "|" + string.Join(";", filterFields) + "|" + string.Join(";", from f in filterValues select f?.ToString() ?? "NULL");
            var result = this.TryGetCachedItem(cacheKey);

            if (result == null)
            {
                result = LookupGuidForMapping(entityName, filterFields, filterValues);

                // We don't cache the absence of a record for the moment as it could be created during the Import - needs further cosnideration, but this isn't a performance bottleneck currently. 
                if (result != Guid.Empty && result != null)
                {
                    this.SetCachedItem(cacheKey, result);
                }
            }

            return result.Value;
        }

        protected override Guid? CreateCachedItem(string cacheKey)
        {
            throw new InvalidOperationException("This cache does not create entries on demand");
        }

        private Guid LookupGuidForMapping(string entityName, string[] filterFields, object[] filterValues)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = entityName,
                ColumnSet = new ColumnSet(false)
            };

            for (int i = 0; i < filterFields.Length; i++)
            {
                if (!(filterValues[i] is EntityReference entRefValue))
                {
                    query.Criteria.AddCondition(filterFields[i], ConditionOperator.Equal, filterValues[i]);
                }
                else
                {
                    Guid refId = entRefValue.Id;
                    query.Criteria.AddCondition(filterFields[i], ConditionOperator.Equal, refId);
                }
            }

            var entities = orgService.GetDataByQuery(query, 2).Entities.ToList();

            if (entities.Count > 1)
            {
                throw new ConfigurationException($"incorrect mapping value - cannot find unique record, Found {entities.Count} maching criteria {entityName}:{string.Join(",", filterFields)}={string.Join(", ", filterValues)}");
            }

            if (entities.Count == 0)
            {
                return Guid.Empty;
            }

            return entities[0].Id;
        }
    }
}