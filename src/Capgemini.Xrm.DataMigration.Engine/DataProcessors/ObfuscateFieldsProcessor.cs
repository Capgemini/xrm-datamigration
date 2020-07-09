﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataScrambler;
using Capgemini.DataScrambler.Scramblers;
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
            if (entity.OperationType != OperationType.Ignore && fieldsToObfuscate != null)
            {
                // Get the List of fields for the current entity if they exist
                Entity originalEntity = entity.OriginalEntity;
                List<FieldToBeObfuscated> fieldsToChangeForCurrentEntity = fieldsToObfuscate.Where(e => e.EntityName == originalEntity.LogicalName).FirstOrDefault().FieldsToBeObfuscated;

                // If the list is not empty process the entities fields
                if (fieldsToChangeForCurrentEntity != null && fieldsToChangeForCurrentEntity.Count > 0)
                {
                    foreach (FieldToBeObfuscated field in fieldsToChangeForCurrentEntity)
                    {
                        if (originalEntity.Attributes.Contains(field.FieldName))
                        {
                            ObfuscateField(originalEntity, field.FieldName);
                        }
                    }
                }
            }
        }

        private void ObfuscateField(Entity entity, string fieldName)
        {
            Type fieldType = entity[fieldName].GetType();
            ICrmObfuscateHandler handler = crmObfuscateHandlers
                    .FirstOrDefault(currentHandler => currentHandler.CanHandle(fieldType));

            if(handler != null)
            {
                handler.HandleObfuscation(entity, fieldName, metaDataCache);
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