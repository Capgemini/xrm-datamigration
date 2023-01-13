using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.DataMigration.Exceptions;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.MappingRules;
using Capgemini.Xrm.DataMigration.Model;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.DataProcessors
{
    public class MapEntityProcessor : IEntityProcessor<Entity, EntityWrapper>
    {
        private readonly ILogger logger;
        private readonly IEntityRepository targetRepo;
        private readonly IEntityMetadataCache metCache;
        private readonly MappingConfiguration mappingConfiguration;
        private readonly Guid targetBusinessUnitId;
        private readonly Guid organizationId;
        private readonly List<string> passOneReferences = new List<string> { "businessunit", "uom", "uomschedule", "queue", "duplicaterule" };

        private List<IMappingRule> entitySpecificRules;

        public MapEntityProcessor(MappingConfiguration mappingConfig, ILogger logger, IEntityRepository targetRepo, List<string> passOneReferences = null)
        {
            mappingConfiguration = mappingConfig ?? new MappingConfiguration();
            targetBusinessUnitId = targetRepo == null ? Guid.Empty : targetRepo.GetParentBuId();
            organizationId = targetRepo == null ? Guid.Empty : targetRepo.GetOrganizationId();
            this.logger = logger;
            this.targetRepo = targetRepo;
            metCache = targetRepo?.GetEntityMetadataCache;
            this.passOneReferences = passOneReferences ?? this.passOneReferences;
        }

        public int MinRequiredPassNumber
        {
            get
            {
                return 1;
            }
        }

        private List<IMappingRule> EntitySpecificRules
        {
            get
            {
                if (entitySpecificRules == null)
                {
                    entitySpecificRules = new List<IMappingRule>()
                    {
                        new BusinessUnitRootRule(targetBusinessUnitId),
                        new MappingAliasedValueRule(targetRepo)
                    };
                }

                return entitySpecificRules;
            }
        }

        public void ProcessEntity(EntityWrapper entity, int passNumber, int maxPassNumber)
        {
            try
            {
                entity.ThrowArgumentNullExceptionIfNull(nameof(entity));

                if (entity.OperationType != OperationType.Ignore)
                {
                    if (entity.LogicalName == "organization")
                    {
                        entity.OriginalEntity.Id = organizationId;
                    }

                    if (mappingConfiguration.Mappings != null && mappingConfiguration.Mappings.Any())
                    {
                        MapGuids(entity);
                    }

                    if (mappingConfiguration.ApplyAliasMapping)
                    {
                        MapEntityId(entity);
                        MapAliasses(entity, passNumber);
                    }

                    // clean aliassed values as they give error on update!
                    RemoveAliassedValues(entity.OriginalEntity);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("MapAliasses: Processing Error", ex);
                throw;
            }
        }

        public void ImportStarted()
        {
            logger.LogInfo($"Executing {nameof(ImportStarted)}");
        }

        public void ImportCompleted()
        {
            logger.LogInfo($"Executing {nameof(ImportCompleted)}");
        }

        private static void MapGuidAttributes(EntityWrapper entity, Dictionary<string, Dictionary<Guid, Guid>> mappings, List<KeyValuePair<string, object>> items)
        {
            foreach (var item in items.Where(item => item.Value is Guid))
            {
                SetEntityAttributeFromMap(entity, mappings, item);
            }
        }

        private static void SetEntityIdFromMap(Dictionary<string, Dictionary<Guid, Guid>> mappings, EntityReference entRef)
        {
            if (mappings != null && mappings.ContainsKey(entRef.LogicalName))
            {
                Dictionary<Guid, Guid> map = mappings[entRef.LogicalName];
                if (map.ContainsKey(entRef.Id))
                {
                    entRef.Id = map[entRef.Id];
                }
            }
        }

        private static List<string> GetAliasedKeys(Entity entity)
        {
            return entity.Attributes.Where(a => a.Value is AliasedValue).Select(a => a.Key).ToList();
        }

        private static void SetEntityAttributeFromMap(EntityWrapper entity, Dictionary<string, Dictionary<Guid, Guid>> mappings, KeyValuePair<string, object> item)
        {
            if (mappings != null && mappings.ContainsKey(entity.LogicalName))
            {
                Guid guidValue = Guid.Parse(item.Value.ToString());

                Dictionary<Guid, Guid> map = mappings[entity.LogicalName];
                if (map.ContainsKey(guidValue))
                {
                    entity.OriginalEntity.Attributes[item.Key] = map[guidValue];
                }
            }
        }

        private void MapGuids(EntityWrapper entity)
        {
            var mappings = mappingConfiguration.Mappings;

            if (mappings != null && mappings.Keys.Any())
            {
                if (entity.IsManyToMany)
                {
                    var items = entity.OriginalEntity.Attributes.Where(p => p.Value is Guid).ToList();

                    MapGuidAttributes(entity, mappings, items);
                }
                else
                {
                    MapNonGuidAttributes(entity, mappings);
                }
            }
        }

        private void MapNonGuidAttributes(EntityWrapper entity, Dictionary<string, Dictionary<Guid, Guid>> mappings)
        {
            foreach (var value in entity.OriginalEntity.Attributes.Select(item => item.Value))
            {
                if (value is EntityReference)
                {
                    EntityReference entRef = value as EntityReference;
                    SetEntityIdFromMap(mappings, entRef);
                }
                else if (value is EntityCollection)
                {
                    EntityCollection entCol = value as EntityCollection;
                    foreach (var ent in entCol.Entities)
                    {
                        MapGuids(new EntityWrapper(ent));
                    }
                }
            }
        }

        private void RemoveAliassedValues(Entity entity)
        {
            List<string> alliasedAttributes = GetAliasedKeys(entity);

            foreach (var fieldToRemove in alliasedAttributes.Where(fieldName => entity.Attributes.ContainsKey(fieldName)))
            {
                entity.Attributes.Remove(fieldToRemove);
            }
        }

        private List<string> GetAliasedKeysForPassOne(Entity entity)
        {
            return entity.Attributes.Where(a => a.Value is AliasedValue
            && passOneReferences.Contains(((AliasedValue)a.Value).EntityLogicalName)).Select(a => a.Key).ToList();
        }

        private Guid FindReplacementValue(EntityWrapper entity, string alias)
        {
            List<AliasedValue> aliasses = entity.OriginalEntity.Attributes
                .Where(s => s.Key.StartsWith(alias, StringComparison.InvariantCulture))
                .Select(a => a.Value as AliasedValue).ToList();

            int pos = 0;

            while (pos < aliasses.Count)
            {
                var item = aliasses[pos];
                string lookUpName = metCache.GetLookUpEntityName(item.EntityLogicalName, item.AttributeLogicalName);

                if (lookUpName == "businessunit" && (string)item.Value == mappingConfiguration.SourceRootBUName)
                {
                    var entRefValue = new EntityReference(lookUpName, targetBusinessUnitId);
                    aliasses[pos] = new AliasedValue(item.EntityLogicalName, item.AttributeLogicalName, entRefValue);
                    logger.LogVerbose($"Found Id:{entRefValue.Id} for entity {entRefValue.LogicalName} by name {entRefValue.Name} for {alias}, RootBU");
                }
                else if (lookUpName != null)
                {
                    var rec = targetRepo.FindEntitiesByName(lookUpName, (string)item.Value);
                    if (rec.Count == 1)
                    {
                        var entRefValue = new EntityReference(lookUpName, rec.First().Id);
                        aliasses[pos] = new AliasedValue(item.EntityLogicalName, item.AttributeLogicalName, entRefValue);
                        logger.LogVerbose($"Found Id:{entRefValue.Id} for entity {entRefValue.LogicalName} by name {entRefValue.Name} for {alias}");
                    }
                    else
                    {
                        logger.LogWarning($"Cannot find unique record, found {rec.Count} for {lookUpName} and Name {item.Value} for {alias}");
                        return Guid.Empty;
                    }
                }

                pos++;
            }

            Guid replaceGuid = Guid.Empty;
            foreach (var rule in EntitySpecificRules)
            {
                try
                {
                    if (rule.ProcessImport(alias, aliasses, out object replaceCandidate))
                    {
                        replaceGuid = (Guid)replaceCandidate;
                        logger.LogVerbose($"FindReplacementValue: Entity:{entity.LogicalName}:{entity.Id} found Guid {replaceGuid} for {alias}");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"FindReplacementValue: Entity:{entity.LogicalName}:{entity.Id}, Rule {rule.GetType().Name} for {alias} Error:{ex.Message}");
                }
            }

            return replaceGuid;
        }

        private void MapEntityId(EntityWrapper entity)
        {
            string entName = entity.LogicalName;
            string idKey = metCache.GetEntityMetadata(entName).PrimaryIdAttribute;
            string idAliasKeys = metCache.GetIdAliasKey(entName);

            List<string> idAliases = GetAliasedKeys(entity.OriginalEntity).Where(s => s.StartsWith(idAliasKeys, StringComparison.InvariantCulture)).ToList();

            var aliasMain = idAliases.Select(s => s.Substring(0, s.LastIndexOf('.'))).ToList();

            foreach (var idAlias in aliasMain.Distinct())
            {
                Guid replaceGuid = FindReplacementValue(entity, idAlias);
                if (replaceGuid != Guid.Empty && entity.Id != replaceGuid)
                {
                    logger.LogInfo($"MapEntityId: Entity:{entName}:{entity.Id} for {idAlias} Id will be replaced with existing one {replaceGuid}!");
                    entity.OriginalEntity.Id = replaceGuid;
                    entity.OperationType = OperationType.Update;

                    if (entity.OriginalEntity.Attributes.ContainsKey(idKey))
                    {
                        entity.OriginalEntity.Attributes.Remove(idKey);
                        logger.LogVerbose($"MapEntityId: Entity:{entName}:{entity.Id} for {idAlias} id attribute {idKey} has been removed.");
                    }
                }
                else if (replaceGuid != Guid.Empty && entity.Id == replaceGuid)
                {
                    entity.OperationType = OperationType.Update;
                    logger.LogVerbose($"MapEntityId: Entity:{entName}:{entity.OriginalEntity.Id} for {idAlias} the same Id detected");
                }
                else if (entity.Id != Guid.Empty)
                {
                    logger.LogInfo($"MapEntityId: Entity:{entName}:{entity.OriginalEntity.Id} Cannot find replacement guid for {idAlias}, existing one will be used.");
                }
                else
                {
                    entity.OriginalEntity.Id = Guid.NewGuid();
                    logger.LogInfo($"MapEntityId: Entity:{entName}:{entity.Id} Cannot find replacement guid for {idAlias}, empty Id Guid, a random GUID {entity.OriginalEntity.Id} is generated.");
                    entity.OperationType = OperationType.Update;
                }
            }

            // Remove all id aliasses
            idAliases.ForEach(key => entity.OriginalEntity.Attributes.Remove(key));
        }

        private void MapAliasses(EntityWrapper entity, int passNumber)
        {
            List<string> aliases = new List<string>();

            if (passNumber == (int)PassType.UpdateLookups)
            {
                aliases = GetAliasedKeys(entity.OriginalEntity);
            }
            else if (passNumber <= (int)PassType.CreateEntity)
            {
                aliases = GetAliasedKeysForPassOne(entity.OriginalEntity);
            }

            var aliasMain = aliases.Select(s => s.Substring(0, s.LastIndexOf('.'))).ToList();

            foreach (var alias in aliasMain.Distinct())
            {
                var replaceGuid = FindReplacementValue(entity, alias);

                var originalFieldName = alias.Split(new char[] { '.' })[1];
                if (replaceGuid == Guid.Empty)
                {
                    logger.LogWarning($"MapAliasses: Entity:{entity.LogicalName}:{entity.Id}, Field:{originalFieldName}, Mapped value not processed correctly for {alias}. cannot find replacement guid, original value will be used if exists or will be empty");
                    continue;
                }

                Type fieldType = metCache.GetAttributeDotNetType(entity.LogicalName, originalFieldName);
                SetReplacementGuid(fieldType, entity, originalFieldName, replaceGuid, alias);
            }

            var entCollections = entity.OriginalEntity.Attributes.Where(a => a.Value is EntityCollection).Select(a => a.Value).ToList();
            entCollections.ForEach(item =>
            {
                EntityCollection entCol = item as EntityCollection;
                foreach (var ent in entCol.Entities)
                {
                    MapAliasses(new EntityWrapper(ent), passNumber);
                }
            });
        }

        private void SetReplacementGuid(Type fieldType, EntityWrapper entity, string originalFieldName, Guid replaceGuid, string alias)
        {
            if (fieldType == typeof(Guid))
            {
                if (entity.OriginalEntity.Attributes.ContainsKey(originalFieldName))
                {
                    logger.LogVerbose($"MapAliasses Guid: Entity:{entity.LogicalName}:{entity.Id}, Field:{originalFieldName}, using replaced Guid {replaceGuid}");
                    entity.OriginalEntity.Attributes[originalFieldName] = replaceGuid;
                }
                else
                {
                    logger.LogVerbose($"MapAliasses Guid: Entity:{entity.LogicalName}:{entity.Id}, Field:{originalFieldName}, only mapping exists, using replaced Guid {replaceGuid}");
                    entity.OriginalEntity.Attributes.Add(originalFieldName, replaceGuid);
                }
            }
            else if (fieldType == typeof(EntityReference))
            {
                if (entity.OriginalEntity.Attributes.ContainsKey(originalFieldName))
                {
                    logger.LogVerbose($"MapAliasses EntRef: Entity:{entity.LogicalName}:{entity.Id}, Field:{originalFieldName}, using replaced Guid {replaceGuid}");
                    var originalEntityReference = (EntityReference)entity.OriginalEntity.Attributes[originalFieldName];
                    originalEntityReference.Id = replaceGuid;

                    if (string.IsNullOrEmpty(originalEntityReference.LogicalName))
                    {
                        originalEntityReference.LogicalName = metCache.GetLookUpEntityName(entity.LogicalName, originalFieldName);
                    }
                }
                else
                {
                    logger.LogVerbose($"MapAliasses EntRef: Entity:{entity.LogicalName}:{entity.Id}, Field:{originalFieldName}, only mapping exists, using replaced Guid {replaceGuid}");
                    string refEntName = metCache.GetLookUpEntityName(entity.LogicalName, originalFieldName);
                    EntityReference entRef = new EntityReference(refEntName, replaceGuid);
                    entity.OriginalEntity.Attributes.Add(originalFieldName, entRef);
                }
            }
            else
            {
                throw new ConfigurationException($"MapAliasses: Bad Data Migrator Configuration! {alias} Using mapping for unsupported type {fieldType.FullName} !");
            }
        }
    }
}