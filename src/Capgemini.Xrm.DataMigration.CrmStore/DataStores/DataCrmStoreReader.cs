﻿using System;
using System.Collections.Generic;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.DataMigration.Core.Model;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.CrmStore.DataStores
{
    public class DataCrmStoreReader : IDataStoreReader<Entity, EntityWrapper>
    {
        private readonly IEntityRepository entityRepo;
        private readonly ILogger logger;
        private readonly int pageSize;
        private readonly int batchSize;
        private readonly int topCount;
        private readonly bool oneEntityPerBatch;
        private readonly List<string> fetchXMLQueries;
        private readonly List<EntityToBeObfuscated> fieldsToObfuscate;

        private int currentPage;
        private string pagingCookie;
        private int currentQueryIdx;
        private int recordsCount;
        private int totalCount;

        public DataCrmStoreReader(ILogger logger, IEntityRepository entityRepo, ICrmStoreReaderConfig readerConfig)
        {
            logger.ThrowIfNull<ArgumentNullException>(nameof(logger));
            entityRepo.ThrowIfNull<ArgumentNullException>(nameof(entityRepo));
            readerConfig.ThrowIfNull<ArgumentNullException>(nameof(readerConfig));

            ReaderConfigValidation(readerConfig.PageSize, readerConfig.BatchSize, readerConfig.TopCount);

            List<string> fetchXmlQueries = entityRepo?.GetEntityMetadataCache != null ? readerConfig?.GetFetchXMLQueries(entityRepo.GetEntityMetadataCache) : null;

            fetchXmlQueries.ThrowArgumentNullExceptionIfNull(nameof(fetchXmlQueries));

            this.entityRepo = entityRepo;
            this.logger = logger;
            pageSize = readerConfig.PageSize;
            batchSize = readerConfig.BatchSize;
            topCount = readerConfig.TopCount;
            oneEntityPerBatch = readerConfig.OneEntityPerBatch;
            fieldsToObfuscate = readerConfig?.FieldsToObfuscate;
            fetchXMLQueries = fetchXmlQueries;
        }

        public IEntityRepository GetEntityRepository => entityRepo;

        public List<EntityToBeObfuscated> GetFieldsToObfuscate => fieldsToObfuscate;

        public List<EntityWrapper> ReadBatchDataFromStore()
        {
            logger.LogVerbose($"DataCrmStoreReader ReadBatchDataFromStore started, queryIndex:{currentQueryIdx}, page{currentPage}");

            var entities = new List<EntityWrapper>();

            if (currentQueryIdx < fetchXMLQueries.Count)
            {
                string fetchXMLQuery = fetchXMLQueries[currentQueryIdx];

                while (entities.Count < batchSize)
                {
                    currentPage++;

                    List<EntityWrapper> pageData = RetrieveEntitiesInPages(entities, fetchXMLQuery);

                    if (pageData.Count < pageSize || recordsCount >= topCount)
                    {
                        UpdatePageTrackingVariables();

                        if (ShouldExitLoop(entities))
                        {
                            break;
                        }

                        fetchXMLQuery = fetchXMLQueries[currentQueryIdx];
                    }
                }
            }

            logger.LogVerbose($"DataCrmStoreReader ReadBatchDataFromStore finished, queryIndex:{currentQueryIdx}, page:{currentPage}, totalCount:{totalCount}");

            return entities;
        }

        public void Reset()
        {
            logger.LogVerbose("DataCrmStoreReader Reset performed");

            currentPage = 0;
            pagingCookie = null;
            currentQueryIdx = 0;
            recordsCount = 0;
            totalCount = 0;
        }

        private static void ReaderConfigValidation(int pageSize, int batchSize, int topCount)
        {
            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Must be more than zero");
            }

            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize), "Must be more than zero");
            }

            if (topCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(topCount), "Must be more than zero");
            }

            if (pageSize > batchSize)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Must be less than or equal to batchSize");
            }

            if (topCount < batchSize)
            {
                throw new ArgumentOutOfRangeException(nameof(topCount), "Must be more than or equal to batchSize");
            }
        }

        private bool ShouldExitLoop(List<EntityWrapper> entities)
        {
            if (currentQueryIdx >= fetchXMLQueries.Count)
            {
                return true;
            }

            if (oneEntityPerBatch && entities.Count > 0)
            {
                return true;
            }

            return false;
        }

        private void UpdatePageTrackingVariables()
        {
            currentQueryIdx++;
            currentPage = 0;
            pagingCookie = null;
            recordsCount = 0;
        }

        private List<EntityWrapper> RetrieveEntitiesInPages(List<EntityWrapper> entities, string fetchXMLQuery)
        {
            List<EntityWrapper> pageData = entityRepo.GetEntitesByFetchXML(fetchXMLQuery, currentPage, pageSize, ref pagingCookie);
            entities.AddRange(pageData);
            recordsCount += pageData.Count;
            totalCount += pageData.Count;

            var entName = string.Empty;

            if (pageData.Count > 0)
            {
                entName = pageData.First().LogicalName;
            }

            logger.LogVerbose($"DataCrmStoreReader retrieved entity:{entName}, page:{currentPage}, query:{currentQueryIdx}, retrievedCount:{pageData.Count}, totalEntityCount:{recordsCount}");
            return pageData;
        }
    }
}