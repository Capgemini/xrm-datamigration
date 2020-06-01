using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.Xrm.DataMigration.DataStore;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.DataProcessors
{
    public class IgnoredFieldsProcessor : IEntityProcessor<Entity, EntityWrapper>
    {
        private readonly List<string> filedsToIgnore;

        public IgnoredFieldsProcessor(List<string> filedsToIgnore)
        {
            this.filedsToIgnore = filedsToIgnore;
        }

        public int MinRequiredPassNumber
        {
            get
            {
                return 1;
            }
        }

        public void ProcessEntity(EntityWrapper entity, int passNumber, int maxPassNumber)
        {
            entity.ThrowArgumentNullExceptionIfNull(nameof(entity));

            if (entity.OperationType != OperationType.Ignore)
            {
                if (filedsToIgnore != null)
                {
                    RemoveIgnoredFieldsFromEntityAttributes(entity);

                    List<EntityCollection> entCol = entity.OriginalEntity.Attributes.Values.OfType<EntityCollection>().ToList();
                    foreach (EntityCollection item in entCol)
                    {
                        foreach (var subEntity in item.Entities)
                        {
                            ProcessEntity(new EntityWrapper(subEntity), passNumber, maxPassNumber);
                        }
                    }
                }

                var tobeDeleted = entity.OriginalEntity.Attributes.Where(p => p.Key.ToUpper(CultureInfo.InvariantCulture).Contains("BE DELETED")).ToList();
                tobeDeleted.ForEach(p => entity.OriginalEntity.Attributes.Remove(p.Key));
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

        private void RemoveIgnoredFieldsFromEntityAttributes(EntityWrapper entity)
        {
            foreach (var fieldName in filedsToIgnore)
            {
                if (entity.OriginalEntity.Attributes.ContainsKey(fieldName))
                {
                    entity.OriginalEntity.Attributes.Remove(fieldName);
                }
            }
        }
    }
}