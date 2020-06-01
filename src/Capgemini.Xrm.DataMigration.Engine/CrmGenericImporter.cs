using System;
using System.Collections.Generic;
using System.Threading;
using Capgemini.DataMigration.Core;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.CrmStore.DataStores;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using Capgemini.Xrm.DataMigration.Model;
using Capgemini.Xrm.DataMigration.Repositories;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine
{
    /// <summary>
    /// Generic Importer to CRM, can use any data store.
    /// Setting processing for ref data, mapping and ignored fields.
    /// </summary>
    public abstract class CrmGenericImporter : GenericCrmDataMigrator
    {
        private readonly ICrmGenericImporterConfig config;
        private readonly IEntityRepository targetEntRepo;

        protected CrmGenericImporter(ILogger logger, IDataStoreReader<Entity, EntityWrapper> storeReader, DataCrmStoreWriter storeWriter, ICrmGenericImporterConfig config)
            : base(logger, storeReader, storeWriter)
        {
            this.config = config;
            targetEntRepo = storeWriter?.GetEntityRepository;
            AddCustomProcessors();
        }

        protected CrmGenericImporter(ILogger logger, IDataStoreReader<Entity, EntityWrapper> storeReader, DataCrmStoreWriter storeWriter, ICrmGenericImporterConfig config, CancellationToken token)
           : base(logger, storeReader, storeWriter, token)
        {
            this.config = config;
            targetEntRepo = storeWriter?.GetEntityRepository;
            AddCustomProcessors();
        }

        public override int GetStartingPassNumber()
        {
            var startingPass = config.PassOneReferences?.Count > 0 ? (int)PassType.CreateRequiredEntity : (int)PassType.CreateEntity;
            Logger.LogVerbose($"CrmGenericImporter GetStartingPassNumber  = {startingPass}");
            return startingPass;
        }

        private void AddCustomProcessors()
        {
            Logger.LogVerbose("CrmFileDataImporter GetProcessors started");

            if (config.PassOneReferences?.Count > 0)
            {
                AddProcessor(new PassZeroReferenceProcessor(config.PassOneReferences, Logger));
            }

            if (config.FiledsToIgnore != null && config.FiledsToIgnore.Count > 0)
            {
                AddProcessor(new IgnoredFieldsProcessor(config.FiledsToIgnore));
            }

            if (config.NoUpdateEntities != null && config.NoUpdateEntities.Count > 0)
            {
                AddProcessor(new NoUpdateProcessor(config.NoUpdateEntities, Logger));
            }

            AddProcessor(new EntityStatusProcessor(config.IgnoreStatuses, config.IgnoreStatusesExceptions));

            AddProcessor(new ReferenceFieldsProcessor(targetEntRepo.GetEntityMetadataCache, config.PassOneReferences));

            AddProcessor(new MapEntityProcessor(config.MigrationConfig, Logger, targetEntRepo, config.PassOneReferences));

            if (config.EntitiesToSync != null && config.EntitiesToSync.Count > 0)
            {
                AddProcessor(new SyncEntitiesProcessor(config.EntitiesToSync, targetEntRepo, Logger));
            }

            AddDeactivationProcessors();
            AddObjectTypeCodeMappingProcessor();

            Logger.LogVerbose("CrmFileDataImporter GetProcessors finished");
        }

        private void AddObjectTypeCodeMappingProcessor()
        {
            if (config.ObjectTypeCodeMappingConfig?.EntityToTypeCodeMapping != null &&
               config.ObjectTypeCodeMappingConfig?.FieldsToSearchForMapping != null)
            {
                AddProcessor(new ObjectTypeCodeProcessor(config.ObjectTypeCodeMappingConfig, Logger, targetEntRepo));
            }
        }

        private void AddDeactivationProcessors()
        {
            if (config.DeactivateAllProcesses || (config.ProcessesToDeactivate != null && config.ProcessesToDeactivate.Count > 0) ||
                (config.PluginsToDeactivate != null && config.PluginsToDeactivate.Count > 0))
            {
                ProcessRepository procRepo = new ProcessRepository(targetEntRepo.GetCurrentOrgService);

                // To prevent deactivating all workflows or plugins if only one list is passed
                if (!config.DeactivateAllProcesses)
                {
                    if (config.ProcessesToDeactivate == null)
                    {
                        config.ResetProcessesToDeactivate();
                    }

                    if (config.PluginsToDeactivate == null)
                    {
                        config.ResetPluginsToDeactivate();
                    }
                }
                else
                {
                    config.ResetProcessesToDeactivate();
                    config.ResetPluginsToDeactivate();
                }

                AddProcessor(new WorkflowsPluginsProcessor(procRepo, Logger, config.PluginsToDeactivate, config.ProcessesToDeactivate));
            }
        }
    }
}