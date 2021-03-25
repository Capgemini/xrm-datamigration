using System;
using System.Collections.Generic;
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
    public class EntityAttributeCache : SimpleMemoryCache<string[]>
    {
        private const int MaxCachedRecords = 50000;
        private const string CacheId = "EA";
        private readonly string entityLogicalName;
        private readonly string[] cacheFields;
        private readonly IOrganizationService orgService;

        public EntityAttributeCache(IOrganizationService orgService, string entityLogicalName, string[] fieldsToCache)
        {
            this.orgService = orgService;
            this.entityLogicalName = entityLogicalName;
            this.cacheFields = fieldsToCache;
        }

        public void LoadAllRecords()
        {
            var query = new QueryExpression(this.entityLogicalName)
            {
                ColumnSet = new ColumnSet(this.cacheFields)
            };

            var results = this.orgService.GetDataByQuery(query, MaxCachedRecords, true, MaxCachedRecords);

            var tempList = new List<string>();

            foreach (var record in results.Entities)
            {
                tempList.Clear();

                foreach (var field in this.cacheFields)
                {
                    tempList.Add(GetFieldValueAsText(record, field));
                }

                this.SetCachedItem(CacheId + record.Id, tempList.ToArray());
            }
        }

        public bool MatchCachedEntityAttributes(Guid entityId, IEnumerable<KeyValuePair<string, string>> attributes)
        {
            var cacheKey = CacheId + entityId;
            var fields = this.GetCachedItem(cacheKey);

            foreach (var a in attributes)
            {
                var fieldIndex = System.Array.IndexOf(this.cacheFields, a.Key);

                if (fieldIndex < 0 || fields[fieldIndex] != a.Value)
                {
                    return false;
                }
            }

            // Only if we have all fields and they are all the same
            return true;
        }

        public static string GetFieldValueAsText(Entity e, string field)
        {
            return GetFieldValueAsText(e.Contains(field) ? e[field] : null);
        }

        public static string GetFieldValueAsText(object val)
        {
            if (val != null)
            {
                if (val is EntityReference)
                {
                    return string.Intern(((EntityReference)val).Id.ToString());
                }
                else if (val is OptionSetValue)
                {
                    return string.Intern(((OptionSetValue)val).Value.ToString());
                }
                else if (val is Money)
                {
                    return ((Money)val).Value.ToString();
                }
                else
                {
                    return val.ToString();
                }
            }

            return null;
        }

        protected override string[] CreateCachedItem(string cacheKey)
        {
            if (cacheKey == null)
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            string entityId = cacheKey.Substring(CacheId.Length);

            var request = new RetrieveRequest
            {
                ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(this.cacheFields),
                Target = new EntityReference(this.entityLogicalName, new Guid(entityId))
            };

            var response = (RetrieveResponse)orgService.Execute(request);

            return (from f in this.cacheFields select response.Entity.Contains(f) ? GetFieldValueAsText(response.Entity, f) : null).ToArray();
        }
    }
}