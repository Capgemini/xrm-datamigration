using System;
using System.Collections.Generic;

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
        /// Allow caalers to reset ProcessesToDeactivate list.
        /// </summary>
        void ResetProcessesToDeactivate();

        /// <summary>
        /// Allow caalers to reset PluginsToDeactivate list.
        /// </summary>
        void ResetPluginsToDeactivate();
    }
}