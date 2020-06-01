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
    /// Exports Data From CRM to Files
    /// No processor required.
    /// </summary>
    public class CrmFileDataExporter : GenericCrmDataMigrator
    {
        public CrmFileDataExporter(ILogger logger, DataCrmStoreReader storeReader, DataFileStoreWriter storeWriter)
            : base(logger, storeReader, storeWriter)
        {
        }

        public CrmFileDataExporter(ILogger logger, DataCrmStoreReader storeReader, DataFileStoreWriter storeWriter, CancellationToken cancellationToken)
            : base(logger, storeReader, storeWriter, cancellationToken)
        {
        }

        public CrmFileDataExporter(ILogger logger, IEntityRepository entityRepo, ICrmStoreReaderConfig readerConfig, IFileStoreWriterConfig writerConfig, CancellationToken token)
            : base(
                logger,
                new DataCrmStoreReader(logger, entityRepo, readerConfig),
                new DataFileStoreWriter(logger, writerConfig),
                token)
        {
        }

        public CrmFileDataExporter(ILogger logger, IEntityRepository entityRepo, CrmExporterConfig exporterConfig, CancellationToken token)
            : base(
                  logger,
                  new DataCrmStoreReader(logger, entityRepo, exporterConfig),
                  new DataFileStoreWriter(logger, exporterConfig),
                  token)
        {
        }
    }
}