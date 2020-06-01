using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Model;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.CrmStore.DataStores
{
    public class DataCrmStoreWriter : IDataStoreWriter<Entity, EntityWrapper>
    {
        private readonly IEntityRepository entityRepo;
        private readonly ILogger logger;
        private readonly List<string> noUpsertEntites;
        private readonly List<string> noUpdateEntities;

        public DataCrmStoreWriter(ILogger logger, IEntityRepository entityRepo, ICrmStoreWriterConfig config, CancellationToken cancellationToken)
            : this(
                  logger,
                  entityRepo,
                  config == null ? 0 : config.SaveBatchSize,
                  config?.NoUpsertEntities,
                  config?.NoUpdateEntities)
        {
            this.CancellationToken = cancellationToken;
        }

        public DataCrmStoreWriter(ILogger logger, IEntityRepository entityRepo, ICrmStoreWriterConfig config)
            : this(
                  logger,
                  entityRepo,
                  config == null ? 0 : config.SaveBatchSize,
                  config?.NoUpsertEntities,
                  config?.NoUpdateEntities)
        {
        }

        public DataCrmStoreWriter(ILogger logger, IEntityRepository entityRepo, int savePageSize = 500, List<string> noUpsertEntities = null, List<string> noUpdateEntities = null)
        {
            logger.ThrowArgumentNullExceptionIfNull(nameof(logger));
            entityRepo.ThrowArgumentNullExceptionIfNull(nameof(entityRepo));
            ExceptionExtensions.ThrowArgumentOutOfRangeExceptionIfTrue(savePageSize <= 0, nameof(savePageSize));

            SavePageSize = savePageSize;
            this.entityRepo = entityRepo;
            this.logger = logger;
            noUpsertEntites = noUpsertEntities;
            this.noUpdateEntities = noUpdateEntities;

            CancellationToken = new CancellationTokenSource().Token;
        }

        public IEntityRepository GetEntityRepository => entityRepo;

        protected ILogger Logger => logger;

        protected CancellationToken CancellationToken { get; set; }

        protected int SavePageSize { get; set; }

        public virtual void SaveBatchDataToStore(List<EntityWrapper> entities)
        {
            entities.ThrowArgumentNullExceptionIfNull(nameof(entities));

            Logger.LogVerbose($"DataCrmStoreWriter SaveBatchDataToStore started, records:{entities.Count} ");
            int pageNo = 1;

            int cnt = 0;
            var pageData = new List<EntityWrapper>();

            foreach (var item in entities)
            {
                pageData.Add(item);
                cnt++;
                if (cnt >= SavePageSize)
                {
                    cnt = 0;

                    PersistPageData(pageNo, pageData, entityRepo);

                    pageData.Clear();
                    pageNo++;
                }
            }

            if (pageData.Count > 0)
            {
                PersistPageData(pageNo, pageData, entityRepo);
            }

            Logger.LogVerbose($"DataCrmStoreWriter SaveBatchDataToStore finished, records:{entities.Count} ");
        }

        public void Reset()
        {
            Logger.LogVerbose("DataCrmStoreWriter Reset performed");
        }

        protected void PersistPageData(int pageNo, List<EntityWrapper> pageData, IEntityRepository repo)
        {
            CancellationToken.ThrowIfCancellationRequested();

            // if ownerid exists then use AssignRequest
            List<EntityWrapper> entitiesWithOwner = pageData.
                Where(p => p.OriginalEntity.Attributes.Contains(EntityFields.OwnerId)).ToList();

            PersistUsingAssignRequest(pageNo, entitiesWithOwner, repo);

            // for many to many UPSERT is not supported! Try Create - update is not supported!
            List<EntityWrapper> manyToManyEntities = pageData.Where(p => p.IsManyToMany
            && p.OperationType != OperationType.Ignore).ToList();

            PersistUsingAssociationRequest(pageNo, manyToManyEntities, repo);

            // for all other allowed entities use UPSERT
            List<EntityWrapper> upsertEntities = pageData.Where(p => !p.IsManyToMany
            && (noUpsertEntites == null || !noUpsertEntites.Contains(p.LogicalName))
            && (noUpdateEntities == null || !noUpdateEntities.Contains(p.LogicalName))
            && p.OperationType != OperationType.Ignore).ToList();

            PersistUsingUpsertRequest(pageNo, upsertEntities, repo);

            // for all other not upsert allowed entities use Update - then if Error Insert
            List<EntityWrapper> noUpsertEntities = pageData.Where(p => !p.IsManyToMany
            && noUpsertEntites != null
            && noUpsertEntites.Contains(p.LogicalName)
            && (noUpdateEntities == null || !noUpdateEntities.Contains(p.LogicalName))
            && p.OperationType != OperationType.Ignore).ToList();

            PersistUsingUpdateCreateRequests(pageNo, noUpsertEntities, repo);

            List<EntityWrapper> updateEntities = pageData
                .Where(p => !p.IsManyToMany
                && noUpdateEntities != null
                && noUpdateEntities.Contains(p.LogicalName)
                && p.OperationType != OperationType.Ignore)
                .ToList();

            PersistUsingCreateRequests(pageNo, updateEntities, repo);
        }

        protected void PersistUsingUpsertRequest(int pageNo, List<EntityWrapper> entities, IEntityRepository repo)
        {
            repo.ThrowArgumentNullExceptionIfNull(nameof(repo));

            if (entities != null && entities.Any())
            {
                List<EntityWrapper> failedEntites = new List<EntityWrapper>();
                repo.CreateUpdateEntities(entities);
                Logger.LogVerbose($"DataCrmStoreWriter PersistUsingUpsertRequest pageNo {pageNo}, count:{entities.Count}");
                foreach (var itemSaved in entities)
                {
                    if (itemSaved.OperationType == OperationType.Failed)
                    {
                        failedEntites.Add(itemSaved);
                        string message = $"DataCrmStoreWriter PersistUsingUpsertRequest : UpSert Entity Issue, trying legacy method {itemSaved.LogicalName}:{itemSaved.Id} - {itemSaved.OperationResult}";
                        Logger.LogVerbose(message);
                    }
                    else
                    {
                        string message = $"DataCrmStoreWriter PersistUsingUpsertRequest : UpSert Entity OK {itemSaved.LogicalName}:{itemSaved.Id} - {itemSaved.OperationResult}";
                        Logger.LogVerbose(message);
                    }
                }

                if (failedEntites.Any())
                {
                    PersistUsingUpdateCreateRequests(pageNo, failedEntites, repo);
                }
            }
        }

        protected void PersistUsingAssociationRequest(int pageNo, List<EntityWrapper> entities, IEntityRepository repo)
        {
            if (entities != null && entities.Any())
            {
                repo.ThrowArgumentNullExceptionIfNull(nameof(repo));

                repo.AssociateManyToManyEntity(entities);
                Logger.LogVerbose($"DataCrmStoreWriter PersistUsingAssociationRequest pageNo {pageNo}, count:{entities.Count}");
                foreach (var itemSaved in entities)
                {
                    if (itemSaved.OperationType == OperationType.Failed
                        && !itemSaved.OperationResult.Contains("Cannot insert duplicate key")
                        && !itemSaved.OperationResult.Contains("Duplicate Record Found for Entity"))
                    {
                        string message = $"DataCrmStoreWriter PersistUsingAssociationRequest : Associate Entity Issue {itemSaved.LogicalName}:{itemSaved.Id} - {itemSaved.OperationResult}";
                        Logger.LogWarning(message);
                    }
                    else
                    {
                        string message = $"DataCrmStoreWriter PersistUsingAssociationRequest : Associate Entity OK {itemSaved.LogicalName}:{itemSaved.Id} - {itemSaved.OperationResult}";
                        Logger.LogVerbose(message);
                    }
                }
            }
        }

        protected void PersistUsingUpdateCreateRequests(int pageNo, List<EntityWrapper> entities, IEntityRepository repo)
        {
            if (entities != null && entities.Any())
            {
                repo.ThrowArgumentNullExceptionIfNull(nameof(repo));

                var messagePrefix = "DataCrmStoreWriter PersistUsingUpdateCreateRequests";

                List<EntityWrapper> failedEntites = new List<EntityWrapper>();
                repo.UpdateEntities(entities);
                Logger.LogVerbose($"{messagePrefix} pageNo {pageNo}, count:{entities.Count}");
                foreach (var itemSaved in entities)
                {
                    string message = $"{messagePrefix} : Update Entity OK {itemSaved.LogicalName}:{itemSaved.Id} - {itemSaved.OperationResult}";

                    if (itemSaved.OperationType == OperationType.Failed)
                    {
                        failedEntites.Add(itemSaved);
                        message = $"{messagePrefix} : Update Entity Issue, trying Create {itemSaved.LogicalName}:{itemSaved.Id} - {itemSaved.OperationResult}";
                    }

                    Logger.LogVerbose(message);
                }

                if (failedEntites.Any())
                {
                    CreateFailedEntities(pageNo, repo, messagePrefix, failedEntites);
                }
            }
        }

        protected void PersistUsingAssignRequest(int pageNo, List<EntityWrapper> entities, IEntityRepository repo)
        {
            if (entities != null && entities.Any())
            {
                repo.ThrowArgumentNullExceptionIfNull(nameof(repo));

                repo.AssignEntities(entities);
                Logger.LogVerbose($"DataCrmStoreWriter PersistUsingAssignRequest pageNo {pageNo}, count:{entities.Count}");

                foreach (var itemSaved in entities)
                {
                    Entity entity = itemSaved.OriginalEntity;

                    if (itemSaved.OperationType == OperationType.Failed)
                    {
                        string message = $"Assign Entity Issue {entity.LogicalName}:{entity.Id} - {itemSaved.OperationResult}";
                        Logger.LogWarning("DataCrmStoreWriter PersistUsingAssignRequest : " + message);
                    }
                    else
                    {
                        string message = $"Assign Entity OK {entity.LogicalName}:{entity.Id} - {itemSaved.OperationResult}";
                        Logger.LogVerbose("DataCrmStoreWriter PersistUsingAssignRequest : " + message);
                    }

                    if (entity.Attributes.ContainsKey(EntityFields.OwnerId))
                    {
                        entity.Attributes.Remove(EntityFields.OwnerId);

                        if (!entity.Attributes.Any())
                        {
                            itemSaved.OperationType = OperationType.Ignore;
                            Logger.LogVerbose($"PersistUsingAssignRequest Entity {entity.LogicalName}:{entity.Id} set to ignored because no other attributes detected");
                        }
                    }
                }
            }
        }

        private void PersistUsingCreateRequests(int pageNo, List<EntityWrapper> entities, IEntityRepository repo)
        {
            if (entities != null && entities.Any())
            {
                repo.CreateEntities(entities);
                Logger.LogVerbose($"{nameof(DataCrmStoreWriter)} {nameof(PersistUsingCreateRequests)} pageNo {pageNo}, count:{entities.Count}");

                foreach (var entity in entities)
                {
                    if (entity.OperationType == OperationType.Failed)
                    {
                        if (entity.OperationErrorCode == -2147220937)
                        {
                            var message = $"Create Entity Exists {entity.LogicalName}:{entity.Id} - {entity.OperationResult}";
                            Logger.LogVerbose($"{nameof(DataCrmStoreWriter)} {nameof(PersistUsingCreateRequests)} : {message}");
                        }
                        else
                        {
                            var message = $"Create Entity Issue {entity.LogicalName}:{entity.Id} - {entity.OperationResult}";
                            Logger.LogWarning($"{nameof(DataCrmStoreWriter)} {nameof(PersistUsingCreateRequests)} : {message}");
                        }
                    }
                    else
                    {
                        var message = $"Create Entity OK {entity.LogicalName}:{entity.Id} - {entity.OperationResult}";
                        Logger.LogVerbose($"{nameof(DataCrmStoreWriter)} {nameof(PersistUsingCreateRequests)} : {message}");
                    }
                }
            }
        }

        private void CreateFailedEntities(int pageNo, IEntityRepository repo, string messagePrefix, List<EntityWrapper> failedEntites)
        {
            repo.CreateEntities(failedEntites);
            Logger.LogVerbose($"{messagePrefix} pageNo {pageNo}, count:{failedEntites.Count}");
            foreach (var itemSaved in failedEntites)
            {
                if (itemSaved.OperationType == OperationType.Failed)
                {
                    string message = $"{messagePrefix} : Create Entity Issue {itemSaved.LogicalName}:{itemSaved.Id} - {itemSaved.OperationResult}";
                    Logger.LogWarning(message);
                }
                else
                {
                    string message = $"{messagePrefix} : Create Entity OK {itemSaved.LogicalName}:{itemSaved.Id} - {itemSaved.OperationResult}";
                    Logger.LogVerbose(message);
                }
            }
        }
    }
}