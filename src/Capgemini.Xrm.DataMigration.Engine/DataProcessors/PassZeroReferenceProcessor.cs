using System.Collections.Generic;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Model;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.DataProcessors
{
    public class PassZeroReferenceProcessor : IEntityProcessor<Entity, EntityWrapper>
    {
        private readonly List<string> passOneReferences;
        private readonly ILogger logger;

        public PassZeroReferenceProcessor(List<string> passOneReferences, ILogger logger)
        {
            this.passOneReferences = passOneReferences;
            this.logger = logger;
        }

        public int MinRequiredPassNumber
        {
            get
            {
                return 0;
            }
        }

        public void ImportCompleted()
        {
            // nothing to do
        }

        public void ImportStarted()
        {
            // nothing to do
        }

        public void ProcessEntity(EntityWrapper entity, int passNumber, int maxPassNumber)
        {
            entity.ThrowArgumentNullExceptionIfNull(nameof(entity));

            if (passNumber == (int)PassType.CreateRequiredEntity)
            {
                if (!passOneReferences.Contains(entity.OriginalEntity.LogicalName))
                {
                    entity.OperationType = OperationType.Ignore;
                    logger.LogVerbose($"{nameof(PassZeroReferenceProcessor)}: Ignoring {entity.OriginalEntity.LogicalName} for pass {passNumber}");
                }
            }

            // ignore in second pass  -already processed!
            else if (passNumber == (int)PassType.CreateEntity && passOneReferences.Contains(entity.OriginalEntity.LogicalName))
            {
                entity.OperationType = OperationType.Ignore;
                logger.LogVerbose($"{nameof(PassZeroReferenceProcessor)}: Ignoring {entity.OriginalEntity.LogicalName} for pass {passNumber}");
            }
        }
    }
}