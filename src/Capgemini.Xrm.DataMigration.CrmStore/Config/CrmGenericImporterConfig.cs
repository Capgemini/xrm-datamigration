using System;
using System.Collections.Generic;
using Capgemini.Xrm.DataMigration.Config;

namespace Capgemini.Xrm.DataMigration.CrmStore.Config
{
    public class CrmGenericImporterConfig : ICrmGenericImporterConfig
    {
        public CrmGenericImporterConfig()
        {
            ProcessesToDeactivate = new List<string>();
            PluginsToDeactivate = new List<Tuple<string, string>>();
            NoUpdateEntities = new List<string>();
            ProcessesToDeactivate = new List<string>();
            PluginsToDeactivate = new List<Tuple<string, string>>();
            IgnoreStatusesExceptions = new List<string>();
            EntitiesToSync = new List<string>();
            FiledsToIgnore = new List<string>();
        }

        public bool DeactivateAllProcesses { get; set; } = false;

        public List<string> EntitiesToSync { get; private set; }

        public List<string> FiledsToIgnore { get; private set; }

        public bool IgnoreStatuses { get; set; } = true;

        public List<string> IgnoreStatusesExceptions { get; private set; }

        public bool IgnoreSystemFields { get; set; } = true;

        public MappingConfiguration MigrationConfig { get; set; } = null;

        public List<Tuple<string, string>> PluginsToDeactivate { get; private set; }

        public List<string> ProcessesToDeactivate { get; private set; }

        public List<string> PassOneReferences { get; private set; } = new List<string> { "businessunit", "uom", "uomschedule", "queue" };

        public ObjectTypeCodeMappingConfiguration ObjectTypeCodeMappingConfiguration { get; set; } = null;

        public List<string> NoUpdateEntities { get; private set; }

        public ObjectTypeCodeMappingConfiguration ObjectTypeCodeMappingConfig { get; set; }

        public Dictionary<string, List<string>> FieldsToObfuscate { get; set; } = null;

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
    }
}