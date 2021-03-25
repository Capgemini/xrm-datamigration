using System;
using System.Collections.Generic;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.Xrm.DataMigration.Cache;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.DataProcessors
{
    public class SkipExistingRecordProcessor : IEntityProcessor<Entity, EntityWrapper>
    {
        private readonly ILogger logger;
        private readonly IEntityRepository targetRepo;
        private Dictionary<string, EntityAttributeCache> cachedRecords = new Dictionary<string, EntityAttributeCache>();

        public SkipExistingRecordProcessor(ILogger logger, IEntityRepository targetRepo)
        {
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

        public void ProcessEntity(EntityWrapper entity, int passNumber, int maxPassNumber)
        {
            try
            {
                entity.ThrowArgumentNullExceptionIfNull(nameof(entity));

                lock (this)
                {
                    if (!this.cachedRecords.ContainsKey(entity.LogicalName))
                    {
                        var cache = new EntityAttributeCache(this.targetRepo.GetCurrentOrgService, entity.LogicalName, (from a in entity.OriginalEntity.Attributes where !(a.Value is AliasedValue) select a.Key).ToArray());
                        cache.LoadAllRecords();
                        this.cachedRecords[entity.LogicalName] = cache;
                    }
                }

                if (entity.OperationType != OperationType.Ignore)
                {
                    lock (this)
                    {
                        var cache = this.cachedRecords[entity.LogicalName];
                        var fields = (from a in entity.OriginalEntity.Attributes select new KeyValuePair<string, string>(a.Key, EntityAttributeCache.GetFieldValueAsText(a.Value))).ToArray();

                        if (cache.MatchCachedEntityAttributes(entity.Id, fields))
                        {
                            entity.OperationType = OperationType.Ignore;
                            logger.LogInfo($"Skipping {entity.LogicalName} {entity.OriginalEntity.Id}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"{GetType().FullName}: Processing Error", ex);
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
    }
}