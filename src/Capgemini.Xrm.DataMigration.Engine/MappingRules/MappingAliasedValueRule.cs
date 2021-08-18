using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Capgemini.Xrm.DataMigration.Core;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.MappingRules
{
    /// <summary>
    /// general mapping rules.
    /// </summary>
    public class MappingAliasedValueRule : IMappingRule
    {
        private readonly IEntityRepository entityRepository;
        private readonly Dictionary<AliasMapCacheKey, Guid> cache;

        public MappingAliasedValueRule(IEntityRepository entityRepository)
        {
            this.entityRepository = entityRepository;
            this.cache = new Dictionary<AliasMapCacheKey, Guid>();
        }

        public bool ProcessImport(string aliasedAttributeName, List<AliasedValue> values, out object replacementValue)
        {
            var entityName = values.Select(p => p.EntityLogicalName).Distinct().Single();
            var attributeNames = values.Select(p => p.AttributeLogicalName).ToArray();
            var attributeValues = values.Select(p => p.Value).ToArray();

            var cacheKey = new AliasMapCacheKey(entityName, attributeNames, attributeValues);
            if (cache.TryGetValue(cacheKey, out Guid cachedValue))
            {
                replacementValue = cachedValue;
            }
            else
            {
                replacementValue = entityRepository.GetGuidForMapping(entityName, attributeNames, attributeValues);

                if ((Guid)replacementValue != Guid.Empty)
                {
                    cache.Add(cacheKey, (Guid)replacementValue);
                }
            }

            return (Guid)replacementValue != Guid.Empty;
        }

        private class AliasMapCacheKey
        {
            public AliasMapCacheKey(string entityName, IEnumerable<string> attributeNames, IEnumerable<object> attributeValues)
            {
                this.EntityName = entityName;
                this.AttributeNames = attributeNames;
                this.AttributeValues = attributeValues;
            }

            public string EntityName { get; private set; }

            public IEnumerable<string> AttributeNames { get; private set; }

            public IEnumerable<object> AttributeValues { get; private set; }

            public override bool Equals(object obj)
            {
                if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                {
                    return false;
                }

                var other = (AliasMapCacheKey)obj;

                return
                    this.EntityName == other.EntityName &&
                    this.AttributeNames.SequenceEqual(other.AttributeNames) &&
                    this.AttributeValues.SequenceEqual(other.AttributeValues);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;

                    hash = (hash * 23) + this.EntityName.GetHashCode();
                    hash = (hash * 23) + ((IStructuralEquatable)this.AttributeNames).GetHashCode(EqualityComparer<string>.Default);
                    hash = (hash * 23) + ((IStructuralEquatable)this.AttributeValues).GetHashCode(EqualityComparer<object>.Default);

                    return hash;
                }
            }
        }
    }
}