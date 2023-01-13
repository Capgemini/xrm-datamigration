using System;
using System.Collections.Generic;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.DataMigration.Core.Model;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.DataProcessors
{
    public class ObfuscateFieldsProcessor : IEntityProcessor<Entity, EntityWrapper>
    {
        private readonly List<EntityToBeObfuscated> fieldsToObfuscate;
        private readonly IEntityMetadataCache metaDataCache;
        private readonly List<ICrmObfuscateHandler> crmObfuscateHandlers;

        public ObfuscateFieldsProcessor(IEntityMetadataCache metaDataCache, List<EntityToBeObfuscated> fieldsToObfuscate)
        {
            this.fieldsToObfuscate = fieldsToObfuscate;
            this.metaDataCache = metaDataCache;
            this.crmObfuscateHandlers = GetObfuscateHandlers();
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
            // Not used
        }

        public void ImportStarted()
        {
            // Not used
        }

        public void ProcessEntity(EntityWrapper entity, int passNumber, int maxPassNumber)
        {
            entity.ThrowArgumentNullExceptionIfNull(nameof(entity));

            if (entity.OperationType != OperationType.Ignore && fieldsToObfuscate != null)
            {
                // Get the List of fields for the current entity if they exist
                Entity originalEntity = entity.OriginalEntity;
                List<FieldToBeObfuscated> fieldsToChangeForCurrentEntity = fieldsToObfuscate.Any(e => e.EntityName == originalEntity.LogicalName) ? fieldsToObfuscate.FirstOrDefault(e => e.EntityName == originalEntity.LogicalName).FieldsToBeObfuscated : null;

                // If the list is not empty process the entities fields
                if (fieldsToChangeForCurrentEntity != null && fieldsToChangeForCurrentEntity.Count > 0)
                {
                    foreach (var fieldToObfuscate in fieldsToChangeForCurrentEntity.Where(field => originalEntity.Attributes.Contains(field.FieldName)))
                    {
                        ObfuscateField(originalEntity, fieldToObfuscate);
                    }
                }
            }
        }

        private void ObfuscateField(Entity entity, FieldToBeObfuscated field)
        {
            Type fieldType = entity[field.FieldName].GetType();
            ICrmObfuscateHandler handler = crmObfuscateHandlers
                    .FirstOrDefault(currentHandler => currentHandler.CanHandle(fieldType));

            if (handler != null)
            {
                handler.HandleObfuscation(entity, field, metaDataCache);
            }
        }

        private List<ICrmObfuscateHandler> GetObfuscateHandlers()
        {
            List<ICrmObfuscateHandler> obfuscateHandlers = new List<ICrmObfuscateHandler>()
            {
                new CrmObfuscateStringHandler(),
                new CrmObfuscateIntHandler(),
                new CrmObfuscateDecimalHandler(),
                new CrmObfuscateDoubleHandler()
            };

            return obfuscateHandlers;
        }
    }
}