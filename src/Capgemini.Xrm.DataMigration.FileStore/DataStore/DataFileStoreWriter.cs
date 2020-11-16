using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataMigration.Exceptions;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.FileStore.Model;
using Capgemini.Xrm.DataMigration.Helpers;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.FileStore.DataStore
{
    public class DataFileStoreWriter : IDataStoreWriter<Entity, EntityWrapper>
    {
        private readonly ILogger logger;
        private readonly string filePrefix;
        private readonly string filesPath;
        private readonly bool seperateFilesPerEntity;
        private readonly List<string> excludedFields;
        private readonly List<EntityToBeObfuscated> fieldsToObfuscate;
        private int currentBatchNo;
        private int fileNo;
        private string lastEntity;

        public DataFileStoreWriter(ILogger logger, IFileStoreWriterConfig config)
            : this(
                  logger,
                  config?.FilePrefix,
                  config?.JsonFolderPath,
                  config?.ExcludedFields,
                  config != null && config.SeperateFilesPerEntity,
                  config?.FieldsToObfuscate)
        {
        }

        public DataFileStoreWriter(ILogger logger, string filePrefix, string filesPath, List<string> excludedFields = null, bool seperateFilesPerEntity = true, List<EntityToBeObfuscated> fieldsToObfuscate = null)
        {
            logger.ThrowArgumentNullExceptionIfNull(nameof(logger));
            filePrefix.ThrowArgumentNullExceptionIfNull(nameof(filePrefix));
            filesPath.ThrowArgumentNullExceptionIfNull(nameof(filesPath));

            this.logger = logger;
            this.filePrefix = filePrefix;
            this.filesPath = filesPath;
            this.seperateFilesPerEntity = seperateFilesPerEntity;
            this.excludedFields = excludedFields;
            this.fieldsToObfuscate = fieldsToObfuscate;
        }

        public void Reset()
        {
            logger.LogInfo("DataFileStoreWriter Reset performed");
            currentBatchNo = 0;
            fileNo = 0;
            lastEntity = null;
        }

        public void SaveBatchDataToStore(List<EntityWrapper> entities)
        {
            entities.ThrowArgumentNullExceptionIfNull(nameof(entities));

            currentBatchNo++;

            logger.LogVerbose($"DataFileStoreWriter SaveBatchDataToStore started, records:{entities.Count}, batchNo:{currentBatchNo}");

            var entitiesToExport = entities.Select(e => new CrmEntityStore(e)).ToList();

            if (excludedFields != null && excludedFields.Any())
            {
                foreach (var item in entitiesToExport)
                {
                    item.Attributes.RemoveAll(p => excludedFields.Contains(p.AttributeName));
                }
            }

            RemoveEntityReferenceNameProperty(entitiesToExport);

            var exportedStore = new CrmExportedDataStore
            {
                RecordsCount = entitiesToExport.Count,
            };
            exportedStore.ExportedEntities.AddRange(entitiesToExport);

            var batchFile = GetFileNameForBatchNo(currentBatchNo, entities[0].LogicalName);

            if (File.Exists(batchFile))
            {
                throw new ConfigurationException($"Store File {batchFile} already exists, clean the store folder first");
            }

            using (FileStream stream = File.OpenWrite(batchFile))
            {
                JsonHelper.Serialize(exportedStore, stream);
            }

            logger.LogVerbose("DataFileStoreWriter SaveBatchDataToStore finished");
        }

        /// <summary>
        /// Remove the value from the name property of any EntityReference type if obfuscation is being performed on the data.
        /// </summary>
        /// <param name="entitiesToExport">Collection of entities being exported.</param>
        public void RemoveEntityReferenceNameProperty(List<CrmEntityStore> entitiesToExport)
        {
            entitiesToExport.ThrowArgumentNullExceptionIfNull(nameof(entitiesToExport));

            if (this.fieldsToObfuscate != null && this.fieldsToObfuscate.Any())
            {
                foreach (var entity in entitiesToExport)
                {
                    foreach (var attr in entity.Attributes.Where(x => x.AttributeType == "Microsoft.Xrm.Sdk.EntityReference"))
                    {
                        ((EntityReference)attr.AttributeValue).Name = null;
                    }
                }
            }
        }

        private string GetFileNameForBatchNo(int batchNo, string entName)
        {
            if (string.IsNullOrEmpty(entName) || !seperateFilesPerEntity)
            {
                return Path.Combine(filesPath, $"{filePrefix}_{batchNo}.json");
            }

            if (string.IsNullOrWhiteSpace(lastEntity) || lastEntity != entName)
            {
                fileNo = 0;
                lastEntity = entName;
            }

            fileNo++;

            return Path.Combine(filesPath, filePrefix + "_" + entName + "_" + fileNo + ".json");
        }
    }
}