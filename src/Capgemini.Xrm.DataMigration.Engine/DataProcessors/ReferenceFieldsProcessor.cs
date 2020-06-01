using System;
using System.Collections.Generic;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Model;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.DataProcessors
{
    /// <summary>
    /// Two passes processor
    /// Pass One - only simple fields, Ignore many to many entities
    /// Pass Two - only references.
    /// </summary>
    public class ReferenceFieldsProcessor : IEntityProcessor<Entity, EntityWrapper>
    {
        private readonly IEntityMetadataCache metCache;
        private readonly List<string> passOneReferences = new List<string> { "businessunit", "uom", "uomschedule", "queue", "duplicaterule" };

        public ReferenceFieldsProcessor(IEntityMetadataCache metCache, List<string> passOneReferences = null)
        {
            this.metCache = metCache;
            this.passOneReferences = passOneReferences ?? this.passOneReferences;
        }

        public int MinRequiredPassNumber
        {
            get
            {
                return 2;
            }
        }

        public void ProcessEntity(EntityWrapper entity, int passNumber, int maxPassNumber)
        {
            entity.ThrowArgumentNullExceptionIfNull(nameof(entity));

            // Only process if ntity is not ignored and pass number = 1 or 2
            if (entity.OperationType != OperationType.Ignore && passNumber <= 2)
            {
                // Ignore many to many and notes in the first attempt
                if (passNumber <= (int)PassType.CreateEntity && (entity.IsManyToMany || entity.OriginalEntity.LogicalName == "annotation"))
                {
                    entity.OperationType = OperationType.Ignore;
                    return;
                }

                // ignore notes, save in full in the second pass
                if (passNumber == (int)PassType.UpdateLookups && entity.OriginalEntity.LogicalName == "annotation")
                {
                    return;
                }

                ProcessReferenceFields(entity, passNumber);
            }
        }

        public void ImportStarted()
        {
            // Do Nothing
        }

        public void ImportCompleted()
        {
            // Do Nothing
        }

        /// <summary>
        /// Removes references fields for pass 1 and all the others for pass 2.
        /// </summary>
        /// <param name="entity">Entity to process.</param>
        /// <param name="passNumber">Pass Number.</param>
        private void ProcessReferenceFields(EntityWrapper entity, int passNumber)
        {
            // Don't process manyTomany in pass 2
            if (entity.IsManyToMany && passNumber == (int)PassType.UpdateLookups)
            {
                return;
            }

            AttributeCollection newCollection = new AttributeCollection();

            foreach (KeyValuePair<string, object> item in entity.OriginalEntity.Attributes)
            {
                var idAliasKey = metCache.GetIdAliasKey(entity.OriginalEntity.LogicalName);

                if (item.Value != null && (item.Value is EntityReference || item.Value is EntityCollection || item.Value is AliasedValue))
                {
                    // For pass 2(Update Lookups) don't filter, add all attributes
                    if (passNumber == (int)PassType.UpdateLookups)
                    {
                        newCollection.Add(item);
                    }

                    // For pass 1 (CreateEntity) EntityReference and Alias
                    AddEntityReferenceAndAliasAttributesToCollection(passNumber, newCollection, item, idAliasKey);
                }
                else if ((passNumber == (int)PassType.CreateRequiredEntity)
                    || (passNumber == (int)PassType.CreateEntity)
                    || item.Key.StartsWith(idAliasKey, StringComparison.InvariantCulture))
                {
                    newCollection.Add(item);
                }
            }

            entity.OriginalEntity.Attributes = newCollection;
        }

        private void AddEntityReferenceAndAliasAttributesToCollection(int passNumber, AttributeCollection newCollection, KeyValuePair<string, object> item, string idAliasKey)
        {
            if (passNumber <= (int)PassType.CreateEntity && (item.Value is EntityReference || item.Value is AliasedValue))
            {
                if (item.Value is EntityReference)
                {
                    EntityReference entRef = item.Value as EntityReference;

                    if (passOneReferences.Contains(entRef.LogicalName))
                    {
                        newCollection.Add(item);
                    }
                }
                else if (item.Value is AliasedValue)
                {
                    AliasedValue aliasedValue = item.Value as AliasedValue;

                    if (passOneReferences.Contains(aliasedValue.EntityLogicalName) || item.Key.StartsWith(idAliasKey, StringComparison.InvariantCulture))
                    {
                        newCollection.Add(item);
                    }
                }
            }
        }
    }
}