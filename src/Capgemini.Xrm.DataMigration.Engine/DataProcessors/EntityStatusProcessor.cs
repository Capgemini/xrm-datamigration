using System.Collections.Generic;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Model;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.DataProcessors
{
    public class EntityStatusProcessor : IEntityProcessor<Entity, EntityWrapper>
    {
        public static readonly IEnumerable<string> StatusFields = new string[] { EntityFields.StateCode, EntityFields.StatusCode };

        private readonly bool ignoreStatuses;
        private readonly IEnumerable<string> entityExceptions;

        public EntityStatusProcessor(bool ignoreStatuses, IEnumerable<string> entityExceptions)
        {
            this.ignoreStatuses = ignoreStatuses;
            this.entityExceptions = entityExceptions ?? new List<string>();
        }

        public int MinRequiredPassNumber
        {
            get
            {
                if (!ignoreStatuses || (entityExceptions != null && entityExceptions.Any()))
                {
                    return (int)PassType.SetRecordStatus;
                }
                else
                {
                    return 0;
                }
            }
        }

        public void ImportCompleted()
        {
            // Go celebrate!!!!
        }

        public void ImportStarted()
        {
            // Get ready
        }

        public void ProcessEntity(EntityWrapper entity, int passNumber, int maxPassNumber)
        {
            entity.ThrowArgumentNullExceptionIfNull(nameof(entity));

            if (entity.OperationType != OperationType.Ignore)
            {
                if (passNumber == (int)PassType.SetRecordStatus)
                {
                    // if ignore statuses and not in exceptions  - do not process, if not ignore status leave as is and remove only when exception
                    if ((ignoreStatuses && !entityExceptions.Contains(entity.LogicalName))
                         || (!ignoreStatuses && entityExceptions.Contains(entity.LogicalName)))
                    {
                        // if not apply status - just ignore
                        entity.OperationType = OperationType.Ignore;
                    }
                    else
                    {
                        RemoveAllButStatusFields(entity);
                    }
                }
                else
                {
                    // in other passes remove statuses
                    RemoveStatusFields(entity);
                }
            }
        }

        private static void RemoveAllButStatusFields(EntityWrapper entity)
        {
            // remove all non id and status fields, so update will conatin only sttaus fields
            var statusAttribs = entity.OriginalEntity.Attributes.Where(a => StatusFields.Contains(a.Key)).ToList();
            entity.OriginalEntity.Attributes.Clear();
            entity.OriginalEntity.Attributes.AddRange(statusAttribs);
        }

        private static void RemoveStatusFields(EntityWrapper entity)
        {
            foreach (var fieldToRemove in StatusFields.Where(fieldName => entity.OriginalEntity.Attributes.ContainsKey(fieldName)))
            {
                entity.OriginalEntity.Attributes.Remove(fieldToRemove);
            }
        }
    }
}