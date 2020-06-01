using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.FileStore.Helpers;
using Capgemini.Xrm.DataMigration.FileStore.Model;
using Capgemini.Xrm.DataMigration.Helpers;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.FileStore.DataStore
{
    public class DataFileStoreReader : IDataStoreReader<Entity, EntityWrapper>
    {
        private readonly ILogger logger;
        private readonly string filePrefix;
        private readonly string filesPath;

        private List<string> filesToRead;
        private int currentBatchNo;

        public DataFileStoreReader(ILogger logger, IFileStoreReaderConfig config)
            : this(logger, config?.FilePrefix, config?.JsonFolderPath)
        {
        }

        public DataFileStoreReader(ILogger logger, string filePrefix, string filesPath)
        {
            logger.ThrowArgumentNullExceptionIfNull(nameof(logger));
            filePrefix.ThrowArgumentNullExceptionIfNull(nameof(filePrefix));
            filesPath.ThrowArgumentNullExceptionIfNull(nameof(filesPath));

            this.logger = logger;
            this.filePrefix = filePrefix;
            this.filesPath = filesPath;
            filesToRead = Directory.GetFiles(filesPath, filePrefix + "*.json").OrderBy(p => p).ToList();
        }

        public void Reset()
        {
            logger.LogInfo("DataFileStoreReader Reset performed");
            filesToRead = Directory.GetFiles(filesPath, $"{filePrefix}*.json").OrderBy(p => p).ToList();
            currentBatchNo = 0;
        }

        public List<EntityWrapper> ReadBatchDataFromStore()
        {
            currentBatchNo++;

            logger.LogVerbose($"DataFileStoreReader ReadBatchDataFromStore started, batchNo:{currentBatchNo}");

            try
            {
                var dataStore = ReadCrmExportedDataStore(currentBatchNo);

                if (dataStore != null)
                {
                    return EntityConverterHelper.ConvertEntities(dataStore.ExportedEntities);
                }

                return new List<EntityWrapper>();
            }
            finally
            {
                logger.LogVerbose($"DataFileStoreReader ReadBatchDataFromStore finsihed, batchNo:{currentBatchNo}");
            }
        }

        private string GetFileNameForBatchNo(int batchNo)
        {
            if (batchNo <= filesToRead.Count)
            {
                int idx = batchNo - 1;
                return filesToRead[idx];
            }

            return string.Empty;
        }

        private CrmExportedDataStore ReadCrmExportedDataStore(int batchNo)
        {
            CrmExportedDataStore exportedStore = null;
            var batchFile = GetFileNameForBatchNo(batchNo);

            if (!string.IsNullOrWhiteSpace(batchFile))
            {
                logger.LogVerbose($"DataFileStoreReader ReadCrmExportedDataStore started, file:{batchFile}, batchNo:{batchNo}");

                if (!File.Exists(batchFile))
                {
                    logger.LogInfo("File does not exist, finishing processing:" + batchFile);
                    return null;
                }

                using (FileStream stream = File.OpenRead(batchFile))
                {
                    exportedStore = JsonHelper.Deserialize<CrmExportedDataStore>(stream);
                }

                logger.LogVerbose($"DataFileStoreReader ReadCrmExportedDataStore finished, loaded {exportedStore.RecordsCount} records");
            }

            return exportedStore;
        }
    }
}