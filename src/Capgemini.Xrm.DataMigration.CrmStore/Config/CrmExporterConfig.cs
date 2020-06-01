using System;
using System.Collections.Generic;
using System.IO;
using Capgemini.DataMigration.Exceptions;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.CrmStore.FetchCreators;
using Newtonsoft.Json;

namespace Capgemini.Xrm.DataMigration.CrmStore.Config
{
    [Serializable]
    public class CrmExporterConfig : ICrmStoreReaderConfig, IFileStoreWriterConfig
    {
        /// <summary>
        /// Gets excluded fields from the output file.
        /// </summary>
        public List<string> ExcludedFields { get; private set; } = new List<string>();

        /// <summary>
        /// Gets or sets folder with fetch XMl files.
        /// </summary>
        public string FetchXMLFolderPath { get; set; }

        /// <summary>
        /// Gets list of CRM SDK Migration Tool schema files.
        /// </summary>
        public List<string> CrmMigrationToolSchemaPaths { get; set; } = new List<string>();

        /// <summary>
        /// Gets used for configuration migration schema file to add filters, ignored for direct fetchXML files.
        /// Is used only when OnlyActiveRecords = false, otherwise ignored.
        /// It will add the filter to generated fetchXML query.
        /// </summary>
        public Dictionary<string, string> CrmMigrationToolSchemaFilters { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets read page size - max 5000.
        /// </summary>
        public int PageSize { get; set; } = 1000;

        /// <summary>
        /// Gets or sets batch Size - how many items stored in one json file.
        /// </summary>
        public int BatchSize { get; set; } = 1000;

        /// <summary>
        /// Gets or sets max Limit of items per entity.
        /// </summary>
        public int TopCount { get; set; } = 10000;

        /// <summary>
        /// Gets or sets a value indicating whether only works with schema file - adds statecode=0 filter.
        /// </summary>
        public bool OnlyActiveRecords { get; set; }

        /// <summary>
        /// Gets or sets json folder with exported files.
        /// </summary>
        public string JsonFolderPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether if true, then one entity type is returned per batch
        /// If false, then multiple entity types are returned.
        /// </summary>
        public bool OneEntityPerBatch { get; set; } = true;

        /// <summary>
        /// Gets or sets preFix for JSON files.
        /// </summary>
        public string FilePrefix { get; set; } = "ExportedData";

        /// <summary>
        /// Gets or sets a value indicating whether seperate files per entity.
        /// </summary>
        public bool SeperateFilesPerEntity { get; set; } = true;

        /// <summary>
        /// Gets mapping for entity refrences, format entityname - field on source entityt - field which hold unique value for mapping (usually name).
        /// </summary>
        public Dictionary<string, Dictionary<string, List<string>>> LookupMapping { get; private set; } =
            new Dictionary<string, Dictionary<string, List<string>>>();

        /// <summary>
        /// Reads configuration from file.
        /// </summary>
        /// <param name="filePath">filePath.</param>
        /// <returns>value.</returns>
        public static CrmExporterConfig GetConfiguration(string filePath)
        {
            FileInfo configFile = new FileInfo(filePath);
            CrmExporterConfig obj;

            if (configFile.Exists)
            {
                obj = GetRootedConfiguration(
                    JsonConvert.DeserializeObject<CrmExporterConfig>(
                        File.ReadAllText(configFile.FullName),
                        JsonSerializerConfig.SerializerSettings),
                    configFile.DirectoryName);
            }
            else
            {
                obj = new CrmExporterConfig();
            }

            return obj;
        }

        /// <summary>
        /// Generates FetchXMLQueries.
        /// </summary>
        /// <returns>value.</returns>
        public List<string> GetFetchXMLQueries()
        {
            List<string> result = new List<string>();

            if (!string.IsNullOrEmpty(FetchXMLFolderPath))
            {
                var dir = new DirectoryInfo(FetchXMLFolderPath);
                var files = dir.GetFiles("*.xml");
                foreach (var file in files)
                {
                    result.Add(File.ReadAllText(file.FullName));
                }
            }

            if (CrmMigrationToolSchemaPaths != null && CrmMigrationToolSchemaPaths.Count > 0)
            {
                foreach (var schemaPath in CrmMigrationToolSchemaPaths)
                {
                    CrmSchemaConfiguration schema = CrmSchemaConfiguration.ReadFromFile(schemaPath);
                    result.AddRange(schema.PrepareFetchXMLFromSchema(OnlyActiveRecords, CrmMigrationToolSchemaFilters, GetMappingFetchCreators()));
                }
            }

            return result;
        }

        /// <summary>
        /// Saves configuration to file.
        /// </summary>
        /// <param name="filePath">filePath.</param>
        public void SaveConfiguration(string filePath)
        {
            if (File.Exists(filePath))
            {
                throw new ConfigurationException($"Config File {filePath} already exists, clean the store folder first");
            }

            string obj = JsonConvert.SerializeObject(this, JsonSerializerConfig.SerializerSettings);
            File.WriteAllText(filePath, obj);
        }

        private static CrmExporterConfig GetRootedConfiguration(CrmExporterConfig config, string rootDirectory)
        {
            for (int i = 0; i < config.CrmMigrationToolSchemaPaths.Count; i++)
            {
                var path = config.CrmMigrationToolSchemaPaths[i];
                if (!string.IsNullOrEmpty(path) && !Path.IsPathRooted(path))
                {
                    config.CrmMigrationToolSchemaPaths[i] = Path.Combine(rootDirectory, path);
                }
            }

            if (!string.IsNullOrEmpty(config.FetchXMLFolderPath) && !Path.IsPathRooted(config.FetchXMLFolderPath))
            {
                config.FetchXMLFolderPath = Path.Combine(rootDirectory, config.FetchXMLFolderPath);
            }

            if (!string.IsNullOrEmpty(config.JsonFolderPath) && !Path.IsPathRooted(config.JsonFolderPath))
            {
                config.JsonFolderPath = Path.Combine(rootDirectory, config.JsonFolderPath);
            }

            return config;
        }

        private List<IMappingFetchCreator> GetMappingFetchCreators()
        {
            var fetchCreators = new List<IMappingFetchCreator>
            {
                new BusinessUnitRootFetchCreator()
            };

            if (LookupMapping != null)
            {
                fetchCreators.Add(new MappingAliasedValueFetchCreator(LookupMapping));
            }

            return fetchCreators;
        }
    }
}