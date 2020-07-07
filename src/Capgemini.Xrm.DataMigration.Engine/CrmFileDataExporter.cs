using System.Threading;
using Capgemini.DataMigration.Core;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.CrmStore.DataStores;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using Capgemini.Xrm.DataMigration.FileStore.DataStore;

namespace Capgemini.Xrm.DataMigration.Engine
{
    /// <summary>
    /// Exports Data From CRM to Files
    /// No processor required.
    /// </summary>
    public class CrmFileDataExporter : GenericCrmDataMigrator
    {
        private IEntityRepository targetEntRepo;
        private DataCrmStoreReader dataCrmStoreReader;

        public CrmFileDataExporter(ILogger logger, DataCrmStoreReader storeReader, DataFileStoreWriter storeWriter)
            : base(logger, storeReader, storeWriter)
        {
            SetConfiguration();
        }

        public CrmFileDataExporter(ILogger logger, DataCrmStoreReader storeReader, DataFileStoreWriter storeWriter, CancellationToken cancellationToken)
            : base(logger, storeReader, storeWriter, cancellationToken)
        {
            SetConfiguration();
        }

        public CrmFileDataExporter(ILogger logger, IEntityRepository entityRepo, ICrmStoreReaderConfig readerConfig, IFileStoreWriterConfig writerConfig, CancellationToken token)
            : base(
                logger,
                new DataCrmStoreReader(logger, entityRepo, readerConfig),
                new DataFileStoreWriter(logger, writerConfig),
                token)
        {
            SetConfiguration();
        }

        public CrmFileDataExporter(ILogger logger, IEntityRepository entityRepo, CrmExporterConfig exporterConfig, CancellationToken token)
            : base(
                  logger,
                  new DataCrmStoreReader(logger, entityRepo, exporterConfig),
                  new DataFileStoreWriter(logger, exporterConfig),
                  token)
        {
            SetConfiguration();
        }

        private void SetConfiguration()
        {
            dataCrmStoreReader = (DataCrmStoreReader)GetStoreReader;
            targetEntRepo = dataCrmStoreReader.GetEntityRepository;
            AddCustomProcessors();
        }

        private void AddCustomProcessors()
        {
            Logger.LogVerbose("CrmFileDataExporter GetProcessors started");

            var fieldsToObfuscate = dataCrmStoreReader.GetFieldsToObfuscate;
            if (fieldsToObfuscate != null && fieldsToObfuscate.Count > 0)
            {
                AddProcessor(new ObfuscateFieldsProcessor(targetEntRepo.GetEntityMetadataCache, fieldsToObfuscate));
                Logger.LogVerbose("Using ObfuscateFieldsProcessor processor");
            }

            Logger.LogVerbose("CrmFileDataImporter GetProcessors finished");
        }
    }
}