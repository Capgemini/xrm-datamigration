using System;
using System.Collections.Generic;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.DataProcessors
{
    public class SyncEntitiesProcessor : IEntityProcessor<Entity, EntityWrapper>
    {
        private readonly List<string> entitiesToSync;
        private readonly IEntityRepository entityRepo;
        private readonly ILogger logger;

        private Dictionary<string, List<Guid>> entitiesToKeep = new Dictionary<string, List<Guid>>();

        public SyncEntitiesProcessor(List<string> entitiesToSync, IEntityRepository entityRepo, ILogger logger)
        {
            this.entitiesToSync = entitiesToSync;
            this.entityRepo = entityRepo;
            this.logger = logger;
        }

        public int MinRequiredPassNumber => 3;

        public void ImportStarted()
        {
            entitiesToKeep = new Dictionary<string, List<Guid>>();
        }

        public void ImportCompleted()
        {
            logger.LogInfo("DeleteEntitiesProcessor ImportCompleted started");

            foreach (var item in entitiesToKeep)
            {
                List<Guid> currentEntities = entityRepo.GetEntitiesByName(item.Key, Array.Empty<string>(), 5000).Select(p => p.Id).ToList();

                List<Guid> entToDelete = currentEntities.Where(p => !item.Value.Contains(p)).ToList();

                foreach (var entId in entToDelete)
                {
                    try
                    {
                        entityRepo.DeleteEntity(item.Key, entId);
                        logger.LogVerbose($"DeleteEntitiesProcessor ImportCompleted Entity deleted:{item.Key},{entId}");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("DeleteEntitiesProcessor ImportCompleted Error", ex);
                    }
                }
            }

            logger.LogInfo("DeleteEntitiesProcessor ImportCompleted finsihed");
        }

        public void ProcessEntity(EntityWrapper entity, int passNumber, int maxPassNumber)
        {
            if (passNumber == maxPassNumber)
            {
                entity.ThrowArgumentNullExceptionIfNull(nameof(entity));

                var entName = entity.OriginalEntity.LogicalName;

                if (entitiesToSync.Contains(entName))
                {
                    if (!entitiesToKeep.ContainsKey(entName))
                    {
                        entitiesToKeep.Add(entName, new List<Guid>());
                    }

                    entitiesToKeep[entName].Add(entity.OriginalEntity.Id);
                }

                entity.OperationType = OperationType.Ignore;
            }
        }
    }
}