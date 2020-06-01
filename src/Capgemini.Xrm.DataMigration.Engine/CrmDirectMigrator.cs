using System.Threading;
using Capgemini.DataMigration.Core;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.CrmStore.DataStores;

namespace Capgemini.Xrm.DataMigration.Engine
{
    /// <summary>
    /// Directly Exports data from one CRM and imports to anther one
    /// Entity processing required.
    /// </summary>
    public class CrmDirectMigrator : CrmGenericImporter
    {
        public CrmDirectMigrator(ILogger logger, DataCrmStoreReader storeReader, DataCrmStoreWriter storeWriter, ICrmGenericImporterConfig config)
            : base(logger, storeReader, storeWriter, config)
        {
        }

        public CrmDirectMigrator(ILogger logger, DataCrmStoreReader storeReader, DataCrmStoreWriter storeWriter, ICrmGenericImporterConfig config, CancellationToken token)
         : base(logger, storeReader, storeWriter, config, token)
        {
        }

        public CrmDirectMigrator(ILogger logger, IEntityRepository entityRepo, ICrmStoreReaderConfig readerConfig, ICrmStoreWriterConfig writerConfig, ICrmGenericImporterConfig importConfig, CancellationToken token)
            : base(
                logger,
                new DataCrmStoreReader(logger, entityRepo, readerConfig),
                new DataCrmStoreWriter(logger, entityRepo, writerConfig, token),
                importConfig,
                token)
        {
        }
    }
}