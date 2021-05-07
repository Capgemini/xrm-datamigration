using System;
using System.Collections.Generic;
using Capgemini.DataMigration.Core.Model;

namespace Capgemini.Xrm.DataMigration.Config
{
    public interface ICrmGenericImporterConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether if true then statecode and statuscode are ignored.
        /// </summary>
        bool IgnoreStatuses { get; set; }

        List<string> FiledsToIgnore { get; }

        /// <summary>
        /// Gets a List of Entities with fields that will be obfuscated.
        /// </summary>
        List<EntityToBeObfuscated> FieldsToObfuscate { get; }

        /// <summary>
        /// Gets list of entity names. If <seealso cref="IgnoreStatuses"/> is true, status wil be apllied to entities listed.
        /// When <seealso cref="IgnoreStatuses"/>  is false, statuses wil be skipped for entities listed here.
        /// </summary>
        List<string> IgnoreStatusesExceptions { get; }

        /// <summary>
        /// Gets a value indicating whether if true, the defined system fields are ignored.
        /// </summary>
        bool IgnoreSystemFields { get; }

        /// <summary>
        /// Gets or sets mapping configuration.
        /// </summary>
        MappingConfiguration MigrationConfig { get; set; }

        /// <summary>
        /// Gets or sets contains the mapping configuration for object type codes.
        /// </summary>
        ObjectTypeCodeMappingConfiguration ObjectTypeCodeMappingConfig { get; set; }

        /// <summary>
        /// Gets all records which are not in the imported data set will be deleted.
        /// </summary>
        List<string> EntitiesToSync { get; }

        /// <summary>
        /// Gets list of processes which will be autmatically deactivatd before Import and activated after.
        /// </summary>
        List<string> ProcessesToDeactivate { get; }

        /// <summary>
        /// Gets or sets a value indicating whether if true, all active plugins and workflows will be deacivated prior to processing and activatedafter import.
        /// </summary>
        bool DeactivateAllProcesses { get; set; }

        /// <summary>
        /// Gets list of plugins which will be autmatically deactivatd before Import and activated after.
        /// </summary>
        List<Tuple<string, string>> PluginsToDeactivate { get; }

        /// <summary>
        /// Gets references set in pass one.
        /// </summary>
        List<string> PassOneReferences { get; }

        /// <summary>
        /// Gets entities that should be created only.
        /// </summary>
        List<string> NoUpdateEntities { get; }

        /// <summary>
        /// Gets or sets a value indicating whether, if true, updates to existing records with correct data are suppressed.
        /// </summary>
        bool SkipExistingRecords { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the repository can cache looked up values, e.g. resolving a username to a guid.
        /// If true this speeds things up considerably, but the flag is here in case of unexpected consequences.
        /// </summary>
        bool EnableLookupCaching { get; set; }

        /// <summary>
        /// Allow caalers to reset ProcessesToDeactivate list.
        /// </summary>
        void ResetProcessesToDeactivate();

        /// <summary>
        /// Allow caalers to reset PluginsToDeactivate list.
        /// </summary>
        void ResetPluginsToDeactivate();
    }
}