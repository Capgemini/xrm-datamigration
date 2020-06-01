using System;
using System.Collections.Generic;
using System.IO;
using Capgemini.DataMigration.Exceptions;
using Capgemini.Xrm.DataMigration.Config;
using Newtonsoft.Json;

namespace Capgemini.Xrm.DataMigration.CrmStore.Config
{
    [Serializable]
    public class CrmImportConfig : IFileStoreReaderConfig, ICrmStoreWriterConfig, ICrmGenericImporterConfig
    {
        private readonly List<string> systemFields = new List<string>
            {
                "createdby",
                "createdonbehalfby",
                "createdon",
                "importsequencenumber",
                "modifiedby",
                "modifiedonbehalfby",
                "modifiedon",
                "owneridtype",
                "owningbusinessunit",
                "owningteam",
                "owninguser",
                "overriddencreatedon",
                "timezoneruleversionnumber",
                "utcconversiontimezonecode",
                "versionnumber",
                "transactioncurrencyid",
                "organizationid"
            };

        private bool ignoreStatuses;
        private bool ignoreSystemFields;

        private List<string> fieldsToIgnoreTotal;

        public CrmImportConfig()
        {
            ProcessesToDeactivate = new List<string>();
            PluginsToDeactivate = new List<Tuple<string, string>>();
            NoUpsertEntities = new List<string>();
            IgnoreStatusesExceptions = new List<string>();
            AdditionalFieldsToIgnore = new List<string>();
            EntitiesToSync = new List<string>();
            NoUpdateEntities = new List<string>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether if true then statecode and statuscode are ignored.
        /// </summary>
        public bool IgnoreStatuses
        {
            get
            {
                return ignoreStatuses;
            }

            set
            {
                ignoreStatuses = value;
                fieldsToIgnoreTotal = null;
            }
        }

        /// <summary>
        /// Gets list of entity names. If <seealso cref="IgnoreStatuses"/> is true, status wil be apllied to entities listed.
        /// When <seealso cref="IgnoreStatuses"/>  is false, statuses wil be skipped for entities listed here.
        /// </summary>
        public List<string> IgnoreStatusesExceptions { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether if true, the defined system fields are ignored.
        /// </summary>
        public bool IgnoreSystemFields
        {
            get
            {
                return ignoreSystemFields;
            }

            set
            {
                ignoreSystemFields = value;
                fieldsToIgnoreTotal = null;
            }
        }

        /// <summary>
        /// Gets or sets mapping configuration.
        /// </summary>
        public MappingConfiguration MigrationConfig { get; set; }

        /// <summary>
        /// Gets final Ignored fields list.
        /// </summary>
        [JsonIgnore]
        public List<string> FiledsToIgnore
        {
            get
            {
                if (fieldsToIgnoreTotal == null)
                {
                    fieldsToIgnoreTotal = AdditionalFieldsToIgnore;

                    if (IgnoreSystemFields)
                    {
                        if (fieldsToIgnoreTotal == null)
                        {
                            fieldsToIgnoreTotal = new List<string>();
                        }

                        foreach (var field in systemFields)
                        {
                            if (!fieldsToIgnoreTotal.Contains(field))
                            {
                                fieldsToIgnoreTotal.Add(field);
                            }
                        }
                    }
                }

                return fieldsToIgnoreTotal;
            }
        }

        public List<string> AdditionalFieldsToIgnore { get; private set; }

        /// <summary>
        /// Gets or sets batch size used for executemultiple request.
        /// </summary>
        public int SaveBatchSize { get; set; } = 500;

        /// <summary>
        /// Gets or sets json folder Path with exported files.
        /// </summary>
        public string JsonFolderPath { get; set; }

        /// <summary>
        /// Gets all records which are not in the imported data set will be deleted.
        /// </summary>
        public List<string> EntitiesToSync { get; private set; }

        /// <summary>
        /// Gets don't use Upsert request, use Create and Update requests instead.
        /// </summary>
        public List<string> NoUpsertEntities { get; private set; }

        /// <summary>
        /// Gets list of plugins which will be autmatically deactivatd before Import and activated after.
        /// </summary>
        public List<Tuple<string, string>> PluginsToDeactivate { get; private set; }

        /// <summary>
        /// Gets list of processes which will be autmatically deactivatd before Import and activated after.
        /// </summary>
        public List<string> ProcessesToDeactivate { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether if true, all active plugins and workflows will be deacivated prior to processing and activatedafter import.
        /// </summary>
        public bool DeactivateAllProcesses { get; set; } = false;

        /// <summary>
        /// Gets or sets preFix for JSON files.
        /// </summary>
        public string FilePrefix { get; set; } = "ExportedData";

        /// <summary>
        /// Gets references set in pass one.
        /// </summary>
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<string> PassOneReferences { get; private set; } = new List<string> { "businessunit", "uom", "uomschedule", "queue" };

        /// <summary>
        /// Gets or sets entity type code mapping configuration.
        /// </summary>
        public ObjectTypeCodeMappingConfiguration ObjectTypeCodeMappingConfig { get; set; }

        /// <summary>
        /// Gets entities that should be created only.
        /// </summary>
        public List<string> NoUpdateEntities { get; private set; }

        /// <summary>
        /// Reads settings from file.
        /// </summary>
        /// <param name="filePath">filePath.</param>
        /// <returns>return value.</returns>
        public static CrmImportConfig GetConfiguration(string filePath)
        {
            FileInfo configFile = new FileInfo(filePath);
            CrmImportConfig obj;

            if (configFile.Exists)
            {
                obj = GetRootedConfiguration(
                    JsonConvert.DeserializeObject<CrmImportConfig>(
                        File.ReadAllText(configFile.FullName),
                        JsonSerializerConfig.SerializerSettings),
                    configFile.DirectoryName);
            }
            else
            {
                obj = new CrmImportConfig();
            }

            return obj;
        }

        /// <summary>
        /// Save settings to file.
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

        /// <summary>
        /// Allow caalers to reset ProcessesToDeactivate list.
        /// </summary>
        public void ResetProcessesToDeactivate()
        {
            ProcessesToDeactivate = new List<string>();
        }

        /// <summary>
        /// Allow caalers to reset PluginsToDeactivate list.
        /// </summary>
        public void ResetPluginsToDeactivate()
        {
            PluginsToDeactivate = new List<Tuple<string, string>>();
        }

        private static CrmImportConfig GetRootedConfiguration(CrmImportConfig config, string rootDirectory)
        {
            if (!string.IsNullOrEmpty(config.JsonFolderPath) && !Path.IsPathRooted(config.JsonFolderPath))
            {
                config.JsonFolderPath = Path.Combine(rootDirectory, config.JsonFolderPath);
            }

            return config;
        }
    }
}