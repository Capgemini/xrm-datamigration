using System;
using System.Linq;
using Capgemini.DataMigration.Cache;
using Capgemini.DataMigration.Exceptions;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace Capgemini.Xrm.DataMigration.Cache
{
    public class EntityMetadataCache : SimpleMemoryCache<EntityMetadata>, IEntityMetadataCache
    {
        private readonly string cacheId = "CRMEntityMetadataCache";

        private readonly IOrganizationService orgService;

        public EntityMetadataCache(IOrganizationService orgService)
        {
            this.orgService = orgService;
        }

        public EntityMetadata GetEntityMetadata(string entityName)
        {
            string cacheKey = $"{cacheId}{entityName}";
            return GetCachedItem(cacheKey);
        }

        public string GetLookUpEntityName(string entityName, string attributeName)
        {
            var ent = GetEntityMetadata(entityName);

            var entRef = ent.ManyToOneRelationships.SingleOrDefault(p => p.ReferencingAttribute == attributeName);

            if (entRef == null)
            {
                return null;
            }

            return entRef.ReferencedEntity;
        }

        /// <summary>
        /// Checks if entity is many to many.
        /// </summary>
        /// <param name="intersectEntityName">intersectEntityName.</param>
        /// <returns>EntityMetadata.IsIntersect or cached value.</returns>
        public ManyToManyDetails GetManyToManyEntityDetails(string intersectEntityName)
        {
            EntityMetadata metadata = GetEntityMetadata(intersectEntityName);

            bool isIntersect = false;

            if (metadata.IsIntersect.HasValue)
            {
                isIntersect = metadata.IsIntersect.Value;
            }

            if (!isIntersect)
            {
                return new ManyToManyDetails() { IsManyToMany = false };
            }

            ManyToManyRelationshipMetadata relationshipDetails = metadata.ManyToManyRelationships.FirstOrDefault(p => p.IntersectEntityName == intersectEntityName);

            return new ManyToManyDetails
            {
                IsManyToMany = true,
                SchemaName = relationshipDetails.SchemaName,
                Entity1IntersectAttribute = relationshipDetails.Entity1IntersectAttribute,
                Entity1LogicalName = relationshipDetails.Entity1LogicalName,
                Entity2IntersectAttribute = relationshipDetails.Entity2IntersectAttribute,
                Entity2LogicalName = relationshipDetails.Entity2LogicalName
            };
        }

        public AttributeMetadata GetAttribute(string entityName, string attributeName)
        {
            var ent = GetEntityMetadata(entityName);
            if (ent == null)
            {
                throw new ConfigurationException($"Entity {entityName} does not exist in CRM!");
            }

            var attr = GetEntityMetadata(entityName)?.Attributes.FirstOrDefault(p => p.LogicalName == attributeName);
            if (attr == null)
            {
                throw new ConfigurationException($"Attribute {attributeName} for Entity {entityName} does not exist in CRM!");
            }

            return attr;
        }

        public Type GetAttributeDotNetType(string entityName, string attributeName)
        {
            var attr = GetAttribute(entityName, attributeName);

            switch (attr.AttributeType.Value)
            {
                case AttributeTypeCode.Boolean:
                    return typeof(bool);

                case AttributeTypeCode.DateTime:
                    return typeof(DateTime);

                case AttributeTypeCode.Decimal:
                    return typeof(decimal);

                case AttributeTypeCode.Double:
                    return typeof(double);

                case AttributeTypeCode.Integer:
                    return typeof(int);

                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.Owner:
                case AttributeTypeCode.Customer:
                    return typeof(EntityReference);

                case AttributeTypeCode.Money:
                    return typeof(Money);

                case AttributeTypeCode.PartyList:
                case AttributeTypeCode.CalendarRules:
                    return typeof(EntityCollection);

                case AttributeTypeCode.Picklist:
                case AttributeTypeCode.State:
                case AttributeTypeCode.Status:
                    return typeof(OptionSetValue);

                case AttributeTypeCode.String:
                case AttributeTypeCode.EntityName:
                case AttributeTypeCode.Memo:
                    return typeof(string);

                case AttributeTypeCode.Uniqueidentifier:
                    return typeof(Guid);

                case AttributeTypeCode.BigInt:
                    return typeof(long);

                case AttributeTypeCode.ManagedProperty:
                case AttributeTypeCode.Virtual:
                    throw new ValidationException($"Not Implemented Attribute type: {attributeName}");

                default:
                    throw new ValidationException($"Unsupported Attribute type: {attributeName}");
            }
        }

        public string GetIdAliasKey(string entName)
        {
            string idKey = GetEntityMetadata(entName).PrimaryIdAttribute;

            string idAliasKey = $"map.{idKey}.";

            return idAliasKey;
        }

        protected override EntityMetadata CreateCachedItem(string cacheKey)
        {
            if (cacheKey == null)
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            string entityName = cacheKey.Substring(cacheId.Length);

            RetrieveEntityRequest request = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.All,
                LogicalName = entityName
            };

            RetrieveEntityResponse response = (RetrieveEntityResponse)orgService.Execute(request);

            return response.EntityMetadata;
        }
    }
}