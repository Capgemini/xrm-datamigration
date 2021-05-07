using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Capgemini.DataMigration.Cache;
using Capgemini.Xrm.DataMigration.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Capgemini.Xrm.DataMigration.Cache
{
    public class EntityAttributeCache : DictionaryCache<string[]>
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

        public static string GetFieldValueAsText(object val)
        {
            if (val is EntityReference)
            {
                return string.Intern(((EntityReference)val).Id.ToString());
            }
            else if (val is OptionSetValue)
            {
                return string.Intern(((OptionSetValue)val).Value.ToString(CultureInfo.InvariantCulture));
            }
            else if (val is Money)
            {
                return ((Money)val).Value.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                return val?.ToString();
            }
        }

        public void LoadAllRecords()
        {
            var query = new QueryExpression(this.entityLogicalName)
            {
                ColumnSet = new ColumnSet(this.cacheFields)
            };

            var results = this.orgService.GetDataByQuery(query, MaxCachedRecords, true, MaxCachedRecords);

            foreach (var record in results.Entities)
            {
                this.SetCachedItem(CacheId + record.Id, GetCacheArray(record));
            }
        }

        public bool MatchCachedEntityAttributes(Guid entityId, IEnumerable<KeyValuePair<string, string>> attributes)
        {
            var cacheKey = CacheId + entityId;
            var fields = this.GetCachedItem(cacheKey);

            if (fields == null || attributes == null)
            {
                return false;   // we have cached that the object does not exist
            }

            foreach (var a in attributes)
            {
                var fieldIndex = Array.IndexOf(this.cacheFields, a.Key);

                if (fieldIndex < 0 || fields[fieldIndex] != a.Value)
                {
                    return false;
                }
            }

            // Only if we have all fields and they are all the same
            return true;
        }

        private static string GetFieldValueAsText(Entity e, string field)
        {
            return GetFieldValueAsText(e.Contains(field) ? e[field] : null);
        }

        private string[] GetCacheArray(Entity entity)
        {
            return (from f in this.cacheFields select entity.Contains(f) ? GetFieldValueAsText(entity, f) : null).ToArray();
        }
    }
}