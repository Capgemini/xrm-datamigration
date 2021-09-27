using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.DataMigration.Exceptions;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.FileStore.Helpers;
using Capgemini.Xrm.DataMigration.FileStore.Model;
using Capgemini.Xrm.DataMigration.Model;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.FileStore.DataStore
{
    public class DataFileStoreWriterCsv : IDataStoreWriter<Entity, EntityWrapper>
    {
        private readonly ILogger logger;
        private readonly string filePrefix;
        private readonly string filesPath;
        private readonly bool seperateFilesPerEntity;
        private readonly string delimiter = ",";
        private readonly CrmSchemaConfiguration schemaConfig;
        private readonly List<string> excludedFields;

        private int currentBatchNo;
        private int fileNo;
        private string lastEntity;

        public DataFileStoreWriterCsv(ILogger logger, IFileStoreWriterConfig config, CrmSchemaConfiguration schemaConfig)
            : this(
                  logger,
                  config?.FilePrefix,
                  config?.JsonFolderPath,
                  config?.ExcludedFields,
                  schemaConfig)
        {
        }

        public DataFileStoreWriterCsv(ILogger logger, string filePrefix, string filesPath, List<string> excludedFields, CrmSchemaConfiguration schemaConfig)
        {
            logger.ThrowArgumentNullExceptionIfNull(nameof(logger));
            filePrefix.ThrowArgumentNullExceptionIfNull(nameof(filePrefix));
            filesPath.ThrowArgumentNullExceptionIfNull(nameof(filesPath));

            if (schemaConfig == null || !schemaConfig.Entities.Any())
            {
                throw new ArgumentNullException(nameof(schemaConfig));
            }

            this.excludedFields = excludedFields;
            this.logger = logger;
            this.filePrefix = filePrefix;
            this.filesPath = filesPath;
            seperateFilesPerEntity = true;
            this.schemaConfig = schemaConfig;

            if (this.excludedFields == null)
            {
                this.excludedFields = new List<string>();
            }
        }

        public void Reset()
        {
            logger.LogInfo("DataFileStoreWriterCsv Reset performed");
            currentBatchNo = 0;
            fileNo = 0;
            lastEntity = null;
        }

        public void SaveBatchDataToStore(List<EntityWrapper> entities)
        {
            entities.ThrowArgumentNullExceptionIfNull(nameof(entities));

            currentBatchNo++;

            logger.LogVerbose($"DataFileStoreWriterCsv SaveBatchDataToStore started, records:{entities.Count}, batchNo:{currentBatchNo}");

            var entitiesToExport = entities.Select(e => new CrmEntityStore(e)).ToList();

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

            List<string> tempCsvFile = new List<string>();

            var header = GetHeader(exportedStore);

            tempCsvFile.Add(CreateCsvHeader(header));

            foreach (CrmEntityStore item in exportedStore.ExportedEntities)
            {
                tempCsvFile.Add(CreateCsvLine(item, header));
            }

            File.WriteAllLines(batchFile, tempCsvFile.ToArray());

            logger.LogVerbose("DataFileStoreWriterCsv SaveBatchDataToStore finished");
        }

        private List<string> GetHeader(CrmExportedDataStore store)
        {
            store.ThrowArgumentNullExceptionIfNull(nameof(store));

            var ent = store.ExportedEntities.FirstOrDefault();
            List<string> header = new List<string>();

            if (!ent.IsManyToMany)
            {
                CrmEntity entity = schemaConfig.Entities.FirstOrDefault(p => p.Name == ent.LogicalName);

                var fields = entity.CrmFields.Where(p => p.FieldName != entity.PrimaryIdField).Select(p => p.FieldName).ToList();

                foreach (var lookup in entity.CrmFields.Where(p => p.FieldName != entity.PrimaryIdField && p.FieldType == "entityreference" && p.FieldName == "ownerid").Select(p => $"{p.FieldName}.LogicalName"))
                {
                    fields.Add(lookup);
                }

                AddFieldWithCheck(header, entity.PrimaryIdField);

                if (fields != null)
                {
                    foreach (var field in fields)
                    {
                        AddFieldWithCheck(header, field);
                    }
                }
            }
            else
            {
                CrmEntity entity = schemaConfig.Entities
                    .First(r => r.CrmRelationships.Select(a => a.RelatedEntityName == ent.LogicalName).Any());

                CrmRelationship rel = entity.CrmRelationships.FirstOrDefault(a => a.RelatedEntityName == ent.LogicalName);

                AddFieldWithCheck(header, $"{rel.RelationshipName}id");
                AddFieldWithCheck(header, entity.PrimaryIdField);
                AddFieldWithCheck(header, rel.TargetEntityPrimaryKey);
            }

            foreach (var item in store.ExportedEntities)
            {
                var mapAtr = item.Attributes.Where(p => p.AttributeType == "Microsoft.Xrm.Sdk.AliasedValue" && !header.Contains(p.AttributeName)).Select(p => p.AttributeName).ToList();

                foreach (var field in mapAtr)
                {
                    AddFieldWithCheck(header, field);
                }
            }

            return header;
        }

        private void AddFieldWithCheck(List<string> header, string fieldName)
        {
            if (!excludedFields.Contains(fieldName))
            {
                header.Add(fieldName);
            }
        }

        private string CreateCsvHeader(List<string> header)
        {
            return string.Join(delimiter, header);
        }

        private string CreateCsvLine(CrmEntityStore record, List<string> header)
        {
            if (record != null)
            {
                string[] items = new string[header.Count];
                items[0] = record.Id.ToString();
                foreach (var item in record.Attributes)
                {
                    int position = header.IndexOf(item.AttributeName);

                    if (position >= 0)
                    {
                        object attrValue = EntityConverterHelper.GetAttributeValueForCsv(item);
                        if (item.AttributeType == "System.String")
                        {
                            string atVal = attrValue?.ToString().Replace("\"", "\"\"");
                            items[position] = $"\"{atVal}\"";
                        }
                        else if (item.AttributeType == "Microsoft.Xrm.Sdk.EntityReference" && item.AttributeName == "ownerid")
                        {
                            var entityref = (EntityReference)item.AttributeValue;
                            int logicalNamePosition = header.IndexOf($"{item.AttributeName}.LogicalName");
                            if (logicalNamePosition >= 0)
                            {
                                items[logicalNamePosition] = $"\"{entityref.LogicalName}\"";
                            }

                            items[position] = attrValue?.ToString();
                        }
                        else
                        {
                            items[position] = attrValue?.ToString();
                        }
                    }
                }

                return string.Join(delimiter, items);
            }

            return null;
        }

        private string GetFileNameForBatchNo(int batchNo, string entName)
        {
            if (string.IsNullOrEmpty(entName) || !seperateFilesPerEntity)
            {
                return Path.Combine(filesPath, $"{filePrefix}_{batchNo}.csv");
            }

            if (string.IsNullOrWhiteSpace(lastEntity) || lastEntity != entName)
            {
                fileNo = 0;
                lastEntity = entName;
            }

            fileNo++;

            return Path.Combine(filesPath, $"{filePrefix}_{entName}_{fileNo}.csv");
        }
    }
}