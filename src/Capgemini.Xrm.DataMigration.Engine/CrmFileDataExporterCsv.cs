using System.Threading;
using Capgemini.DataMigration.Core;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.CrmStore.DataStores;
using Capgemini.Xrm.DataMigration.FileStore.DataStore;

namespace Capgemini.Xrm.DataMigration.Engine
{
    /// <summary>
    /// Exports Data From CRM to Files.
    /// No processor required.
    /// </summary>
    public class CrmFileDataExporterCsv : GenericCrmDataMigrator
    {
        public CrmFileDataExporterCsv(ILogger logger, DataCrmStoreReader storeReader, DataFileStoreWriterCsv storeWriter)
            : base(logger, storeReader, storeWriter)
        {
        }

        public CrmFileDataExporterCsv(ILogger logger, DataCrmStoreReader storeReader, DataFileStoreWriterCsv storeWriter, CancellationToken cancellationToken)
            : base(logger, storeReader, storeWriter, cancellationToken)
        {
        }

        public CrmFileDataExporterCsv(ILogger logger, IEntityRepository entityRepo, ICrmStoreReaderConfig readerConfig, IFileStoreWriterConfig writerConfig, CrmSchemaConfiguration schemaConfig, CancellationToken token)
            : base(
                logger,
                new DataCrmStoreReader(logger, entityRepo, readerConfig),
                new DataFileStoreWriterCsv(logger, writerConfig, schemaConfig),
                token)
        {
        }

        public CrmFileDataExporterCsv(ILogger logger, IEntityRepository entityRepo, CrmExporterConfig exporterConfig, CrmSchemaConfiguration schemaConfig, CancellationToken token)
            : base(
              logger,
              new DataCrmStoreReader(logger, entityRepo, exporterConfig),
              new DataFileStoreWriterCsv(logger, exporterConfig, schemaConfig),
              token)
        {
        }
    }
}