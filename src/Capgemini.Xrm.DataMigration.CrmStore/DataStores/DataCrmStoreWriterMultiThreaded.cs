using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;

namespace Capgemini.Xrm.DataMigration.CrmStore.DataStores
{
    /// <summary>
    /// MultiThreaded CRM writer - each EntityRepo must use different instance of OrganizationalSerice
    /// So many tasks started as many EntityRepos passed.
    /// </summary>
    public class DataCrmStoreWriterMultiThreaded : DataCrmStoreWriter
    {
        private readonly List<IEntityRepository> entityRepos;
        private readonly List<Task> currentTasks = new List<Task>();
        private readonly int maxThreads;

        public DataCrmStoreWriterMultiThreaded(ILogger logger, List<IEntityRepository> entityRepos, ICrmStoreWriterConfig config, CancellationToken cancellationToken)
            : this(logger, entityRepos, config)
        {
            this.CancellationToken = cancellationToken;
        }

        public DataCrmStoreWriterMultiThreaded(ILogger logger, List<IEntityRepository> entityRepos, ICrmStoreWriterConfig config)
            : this(
                  logger,
                  entityRepos,
                  config == null ? 0 : config.SaveBatchSize,
                  config?.NoUpsertEntities,
                  config?.NoUpdateEntities)
        {
        }

        public DataCrmStoreWriterMultiThreaded(ILogger logger, List<IEntityRepository> entityRepos, int savePageSize = 500, List<string> noUpsertEntities = null, List<string> noUpdateEntities = null)
            : base(logger, entityRepos.FirstOrDefault(), savePageSize, noUpsertEntities, noUpdateEntities)
        {
            entityRepos.ThrowArgumentNullExceptionIfNull(nameof(entityRepos));

            this.entityRepos = entityRepos;
            maxThreads = entityRepos.Count;
        }

        public override void SaveBatchDataToStore(List<EntityWrapper> entities)
        {
            entities.ThrowArgumentNullExceptionIfNull(nameof(entities));

            int currentRepoPos = 0;
            currentTasks.Clear();

            Logger.LogVerbose($"DataCrmStoreWriterMultiThreaded SaveBatchDataToStore started, records:{entities.Count} ");
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

                    if (currentRepoPos >= maxThreads)
                    {
                        currentRepoPos = 0;
                    }

                    var repo = entityRepos[currentRepoPos];
                    var dataToProcess = pageData.ToList();
                    var pageNoToProcess = pageNo;

                    currentRepoPos++;

                    var newTask = Task.Run(() => PersistPageData(pageNoToProcess, dataToProcess, repo), CancellationToken);
                    currentTasks.Add(newTask);

                    if (currentTasks.Count >= maxThreads)
                    {
                        Task.WaitAny(currentTasks.ToArray());
                    }

                    currentTasks.RemoveAll(t => t.IsCompleted);

                    pageData = new List<EntityWrapper>();
                    pageNo++;
                }
            }

            if (pageData.Count > 0)
            {
                if (currentRepoPos >= maxThreads)
                {
                    currentRepoPos = 0;
                }

                var repo = entityRepos[currentRepoPos];

                PersistPageData(pageNo, pageData, repo);
            }

            Task.WaitAll(currentTasks.ToArray());
            Logger.LogVerbose($"DataCrmStoreWriterMultiThreaded SaveBatchDataToStore finished, records:{entities.Count} ");
        }
    }
}