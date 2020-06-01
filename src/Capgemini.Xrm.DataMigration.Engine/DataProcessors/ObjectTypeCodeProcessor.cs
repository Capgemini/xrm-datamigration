using System.Collections.Generic;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace Capgemini.Xrm.DataMigration.Engine.DataProcessors
{
    public class ObjectTypeCodeProcessor : IEntityProcessor<Entity, EntityWrapper>
    {
        private readonly ObjectTypeCodeMappingConfiguration objectTypeCodeMappingConfig;
        private readonly ILogger logger;
        private readonly IEntityRepository targetRepo;

        public ObjectTypeCodeProcessor(ObjectTypeCodeMappingConfiguration objectTypeCodeMappingConfig, ILogger logger, IEntityRepository targetRepo)
        {
            this.objectTypeCodeMappingConfig = objectTypeCodeMappingConfig;
            this.logger = logger;
            this.targetRepo = targetRepo;
        }

        public int MinRequiredPassNumber
        {
            get
            {
                return 1;
            }
        }

        public void ImportCompleted()
        {
            logger.LogInfo($"Executing {nameof(ImportCompleted)}");
        }

        public void ImportStarted()
        {
            logger.LogInfo($"Executing {nameof(ImportStarted)}");
        }

        public void ProcessEntity(EntityWrapper entity, int passNumber, int maxPassNumber)
        {
            entity.ThrowArgumentNullExceptionIfNull(nameof(entity));

            Entity originalEntity = entity.OriginalEntity;
            objectTypeCodeMappingConfig.FieldsToSearchForMapping.ForEach(field => PerformMappingOnSingleField(originalEntity, field));
        }

        private void PerformMappingOnSingleField(Entity originalEntity, string field)
        {
            if (originalEntity.Attributes.Contains(field))
            {
                int fieldValue = originalEntity.GetAttributeValue<int>(field);

                if (objectTypeCodeMappingConfig.EntityToTypeCodeMapping.Values.Contains(fieldValue))
                {
                    // switch the keys and values before search
                    Dictionary<int, string> typeCodeToEntityMapping = objectTypeCodeMappingConfig.EntityToTypeCodeMapping.ToDictionary(e => e.Value, e => e.Key);

                    // Get the entity logical name
                    string entityToGetTypeCodeOf = typeCodeToEntityMapping[fieldValue];

                    // Retrieve the type code value for the entity
                    EntityMetadata metaDataCache = targetRepo.GetEntityMetadataCache.GetEntityMetadata(entityToGetTypeCodeOf);
                    int? retrievedTypeCode = metaDataCache.ObjectTypeCode;

                    if (retrievedTypeCode != null)
                    {
                        // Switch the retrieved Value
                        originalEntity.Attributes[field] = retrievedTypeCode.Value;
                    }
                }
            }
        }
    }
}