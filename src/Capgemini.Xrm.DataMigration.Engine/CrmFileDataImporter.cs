using System.Collections.Generic;
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
    /// Imports data from Files to CRM.
    /// </summary>
    public class CrmFileDataImporter : CrmGenericImporter
    {
        public CrmFileDataImporter(ILogger logger, DataFileStoreReader storeReader, DataCrmStoreWriter storeWriter, ICrmGenericImporterConfig config)
            : base(logger, storeReader, storeWriter, config)
        {
        }

        public CrmFileDataImporter(ILogger logger, DataFileStoreReader storeReader, DataCrmStoreWriter storeWriter, ICrmGenericImporterConfig config, CancellationToken token)
         : base(logger, storeReader, storeWriter, config, token)
        {
        }

        public CrmFileDataImporter(ILogger logger, IEntityRepository entityRepo, IFileStoreReaderConfig readerConfig, ICrmStoreWriterConfig writerConfig, ICrmGenericImporterConfig importConfig, CancellationToken token)
            : base(
                logger,
                new DataFileStoreReader(logger, readerConfig),
                new DataCrmStoreWriter(logger, entityRepo, writerConfig, token),
                importConfig,
                token)
        {
        }

        public CrmFileDataImporter(ILogger logger, IEntityRepository entityRepo, CrmImportConfig importConfig, CancellationToken token)
          : base(
              logger,
              new DataFileStoreReader(logger, importConfig),
              new DataCrmStoreWriter(logger, entityRepo, importConfig, token),
              importConfig,
              token)
        {
        }

        public CrmFileDataImporter(ILogger logger, List<IEntityRepository> entityRepos, IFileStoreReaderConfig readerConfig, ICrmStoreWriterConfig writerConfig, ICrmGenericImporterConfig importConfig, CancellationToken token)
          : base(
              logger,
              new DataFileStoreReader(logger, readerConfig),
              new DataCrmStoreWriterMultiThreaded(logger, entityRepos, writerConfig, token),
              importConfig,
              token)
        {
        }

        public CrmFileDataImporter(ILogger logger, List<IEntityRepository> entityRepos, CrmImportConfig importConfig, CancellationToken token)
         : base(
               logger,
               new DataFileStoreReader(logger, importConfig),
               new DataCrmStoreWriterMultiThreaded(logger, entityRepos, importConfig, token),
               importConfig,
               token)
        {
        }
    }
}