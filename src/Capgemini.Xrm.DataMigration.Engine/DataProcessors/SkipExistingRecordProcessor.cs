using System;
using System.Collections.Generic;
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
using Microsoft.Xrm.Sdk.Query;
using Capgemini.Xrm.DataMigration.Extensions;

namespace Capgemini.Xrm.DataMigration.Engine.DataProcessors
{
    public class SkipExistingRecordProcessor : IEntityProcessor<Entity, EntityWrapper>
    {
        private const int MaxCachedrecords = 20000;
        private readonly ILogger logger;
        private readonly IEntityRepository targetRepo;
        private Dictionary<string, Tuple<string[], Dictionary<Guid, KeyValuePair<string, string>[]>>> cachedRecordKeys = new Dictionary<string, Tuple<string[], Dictionary<Guid, KeyValuePair<string, string>[]>>>();

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
                    if (!this.cachedRecordKeys.ContainsKey(entity.LogicalName))
                    {
                        var fieldsToMatch = (from a in entity.OriginalEntity.Attributes where !(a.Value is AliasedValue) select a.Key).ToArray();
                        var recordCache = new Dictionary<Guid, KeyValuePair<string, string>[]>();
                        this.cachedRecordKeys[entity.LogicalName] = new Tuple<string[], Dictionary<Guid, KeyValuePair<string, string>[]>>(fieldsToMatch, recordCache);

                        var query = new QueryExpression(entity.LogicalName)
                        {
                            ColumnSet = new ColumnSet(fieldsToMatch.ToArray())
                        };

                        var results = this.targetRepo.GetCurrentOrgService.GetDataByQuery(query, MaxCachedrecords, true, MaxCachedrecords);

                        if (results == null)
                        {
                            this.logger.LogWarning("Record skipping skipped, too many records returned");
                        }
                        else
                        {
                            var tempList = new List<KeyValuePair<string, string>>();

                            foreach (var record in results.Entities)
                            {
                                tempList.Clear();

                                foreach (var field in fieldsToMatch)
                                {
                                    tempList.Add(new KeyValuePair<string, string>(field, GetFieldValueAsText(record, field)));
                                }

                                recordCache[record.Id] = tempList.ToArray();
                            }

                            this.logger.LogInfo($"Cached {recordCache.Count} records of type {entity.LogicalName} for skipping {GetType().FullName}");
                        }
                    }
                }

                if (entity.OperationType != OperationType.Ignore)
                {
                    lock (this)
                    {
                        if (this.cachedRecordKeys[entity.LogicalName].Item2.TryGetValue(entity.OriginalEntity.Id, out KeyValuePair<string, string>[] fields))
                        {
                            var allFound = true;
                            var fieldsToMatch = this.cachedRecordKeys[entity.LogicalName].Item1;

                            foreach (var attr in entity.OriginalEntity.Attributes)
                            {
                                if (fieldsToMatch.Contains(attr.Key))
                                {
                                    var val = GetFieldValueAsText(entity.OriginalEntity, attr.Key);
                                    var found = (from f in fields where f.Key == attr.Key && f.Value == val select f).Any();
                                    allFound = allFound && found;
                                }
                            }

                            if (allFound)
                            {
                                entity.OperationType = OperationType.Ignore;
                                logger.LogInfo($"Skipping {entity.LogicalName} {entity.OriginalEntity.Id}");
                            }
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

        private string GetFieldValueAsText(Entity e, string field)
        {
            object val = e.Contains(field) ? e[field] : null;

            if (val != null)
            {
                if (val is EntityReference)
                {
                    return ((EntityReference)val).Id.ToString();
                }
                else if (val is OptionSetValue)
                {
                    return ((OptionSetValue)val).Value.ToString();
                }
                else if (val is Money)
                {
                    return ((Money)val).Value.ToString();
                }
                else
                {
                    return val.ToString();
                }
            }

            return null;
        }
    }
}