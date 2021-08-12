using System.Collections.Generic;
using System.IO;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.FileStore.Helpers;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.FileStore.DataStore
{
    public class DataFileStoreReaderCsv : IDataStoreReader<Entity, EntityWrapper>
    {
        private readonly ILogger logger;
        private readonly string filePrefix;
        private readonly string filesPath;
        private readonly ReadCsvHelper csvHelper;

        private List<string> filesToRead;
        private int currentBatchNo;

        public DataFileStoreReaderCsv(ILogger logger, IFileStoreReaderConfig config, CrmSchemaConfiguration schemaConfig)
            : this(
                  logger,
                  config?.FilePrefix,
                  config?.JsonFolderPath,
                  schemaConfig)
        {
        }

        public DataFileStoreReaderCsv(ILogger logger, string filePrefix, string filesPath, CrmSchemaConfiguration schemaConfig)
        {
            logger.ThrowArgumentNullExceptionIfNull(nameof(logger));
            filePrefix.ThrowArgumentNullExceptionIfNull(nameof(filePrefix));
            filesPath.ThrowArgumentNullExceptionIfNull(nameof(filesPath));

            this.logger = logger;
            this.filePrefix = filePrefix;
            this.filesPath = filesPath;
            filesToRead = Directory.GetFiles(filesPath, $"{filePrefix}*.csv").OrderBy(p => p).ToList();
            csvHelper = new ReadCsvHelper(schemaConfig);
        }

        public void Reset()
        {
            logger.LogInfo("DataFileStoreReaderCsv Reset performed");
            filesToRead = Directory.GetFiles(filesPath, $"{filePrefix}*.csv").OrderBy(p => p).ToList();
            currentBatchNo = 0;
        }

        public List<EntityWrapper> ReadBatchDataFromStore()
        {
            currentBatchNo++;
            logger.LogVerbose($"DataFileStoreReaderCsv ReadBatchDataFromStore started, batchNo:{currentBatchNo}");

            try
            {
                var dataStore = ReadCrmExportedDataStore(currentBatchNo);

                if (dataStore != null)
                {
                    return dataStore;
                }

                return new List<EntityWrapper>();
            }
            finally
            {
                logger.LogVerbose($"DataFileStoreReaderCsv ReadBatchDataFromStore finsihed, batchNo:{currentBatchNo}");
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

        private List<EntityWrapper> ReadCrmExportedDataStore(int batchNo)
        {
            List<EntityWrapper> exportedStore = null;

            var batchFile = GetFileNameForBatchNo(batchNo);

            if (!string.IsNullOrWhiteSpace(batchFile))
            {
                logger.LogVerbose($"DataFileStoreReaderCsv ReadCrmExportedDataStore started, file:{batchFile}, batchNo:{batchNo}");

                if (!File.Exists(batchFile))
                {
                    logger.LogInfo($"File does not exist, finishing processing:{batchFile}");
                    return new List<EntityWrapper>();
                }

                exportedStore = csvHelper.ReadFromFile(batchFile, GetEntityNameFromFileName(batchFile));

                logger.LogVerbose($"DataFileStoreReaderCsv ReadCrmExportedDataStore finished, loaded {exportedStore.Count} records");
            }

            return exportedStore;
        }

        private string GetEntityNameFromFileName(string batchFile)
        {
            FileInfo file = new FileInfo(batchFile);
            string fileName = file.Name;

            string entName = fileName.Replace($"{filePrefix}_", string.Empty);

            int lastUnderScore = entName.LastIndexOf('_');

            entName = entName.Substring(0, lastUnderScore);

            return entName;
        }
    }
}