using System;
using System.Collections.Generic;
using Capgemini.DataMigration.Core.Model;
using Capgemini.Xrm.DataMigration.Config;

namespace Capgemini.Xrm.DataMigration.CrmStore.Config
{
    public class CrmGenericImporterConfig : ICrmGenericImporterConfig
    {
        public CrmGenericImporterConfig()
        {
        }

        public bool DeactivateAllProcesses { get; set; }

        public List<string> EntitiesToSync { get; private set; } = new List<string>();

        public List<string> FiledsToIgnore { get; private set; } = new List<string>();

        public bool IgnoreStatuses { get; set; } = true;

        public List<string> IgnoreStatusesExceptions { get; private set; } = new List<string>();

        public bool IgnoreSystemFields { get; set; } = true;

        public MappingConfiguration MigrationConfig { get; set; }

        public List<Tuple<string, string>> PluginsToDeactivate { get; private set; } = new List<Tuple<string, string>>();

        public List<string> ProcessesToDeactivate { get; private set; } = new List<string>();

        public List<string> PassOneReferences { get; private set; } = new List<string> { "businessunit", "uom", "uomschedule", "queue" };

        public ObjectTypeCodeMappingConfiguration ObjectTypeCodeMappingConfiguration { get; set; }

        public List<string> NoUpdateEntities { get; private set; } = new List<string>();

        public ObjectTypeCodeMappingConfiguration ObjectTypeCodeMappingConfig { get; set; }

        public List<EntityToBeObfuscated> FieldsToObfuscate { get; private set; } = new List<EntityToBeObfuscated>();

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