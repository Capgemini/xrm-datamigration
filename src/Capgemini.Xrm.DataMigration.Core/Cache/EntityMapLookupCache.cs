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
    public class EntityMapLookupCache : DictionaryCache<Guid?>
    {
        private const string CacheId = "CRMMapCache";
        private readonly IOrganizationService orgService;

        public EntityMapLookupCache(IOrganizationService orgService)
        {
            this.orgService = orgService;
        }

        public Guid GetGuidForMapping(string entityName, string[] filterFields, object[] filterValues, Func<string, string[], object[], Guid> implementation)
        {
            if (filterFields == null || filterValues == null || filterFields.Length != filterValues.Length)
            {
                throw new ArgumentException("Filter fields must have same length as filter values!");
            }

            if (implementation == null)
            {
                throw new ArgumentException("Implementation must be provided");
            }

            var filterFieldsText = string.Join(";", filterFields);
            var filterValuesText = string.Join(";", from f in filterValues select f?.ToString() ?? "NULL");
            var cacheKey = $"{CacheId}|{entityName}|{filterFieldsText}|{filterValuesText}";
            var result = this.GetCachedItem(cacheKey);

            if (result == null)
            {
                result = implementation(entityName, filterFields, filterValues);

                // We don't cache the absence of a record for the moment as it could be created during the Import - needs further cosnideration, but this isn't a performance bottleneck currently.
                if (result != Guid.Empty && result != null)
                {
                    this.SetCachedItem(cacheKey, result);
                }
            }

            return result.Value;
        }
    }
}