using System.Collections.Generic;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Model;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.DataProcessors
{
    public class NoUpdateProcessor : IEntityProcessor<Entity, EntityWrapper>
    {
        private readonly List<string> noUpdateEntities;
        private readonly ILogger logger;

        public NoUpdateProcessor(List<string> noUpdateEntities, ILogger logger)
        {
            this.noUpdateEntities = noUpdateEntities;
            this.logger = logger;
        }

        public int MinRequiredPassNumber
        {
            get
            {
                return 1;
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

        public void ProcessEntity(EntityWrapper entity, int passNumber, int maxPassNumber)
        {
            entity.ThrowArgumentNullExceptionIfNull(nameof(entity));

            if (noUpdateEntities.Contains(entity.LogicalName) && (passNumber == (int)PassType.CreateRequiredEntity || passNumber == (int)PassType.CreateEntity))
            {
                entity.OperationType = OperationType.Create;
            }
        }
    }
}