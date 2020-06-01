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
    public class CrmFileDataImporterCsv : CrmGenericImporter
    {
        public CrmFileDataImporterCsv(ILogger logger, DataFileStoreReaderCsv storeReader, DataCrmStoreWriter storeWriter, ICrmGenericImporterConfig config)
            : base(logger, storeReader, storeWriter, config)
        {
        }

        public CrmFileDataImporterCsv(ILogger logger, DataFileStoreReaderCsv storeReader, DataCrmStoreWriter storeWriter, ICrmGenericImporterConfig config, CancellationToken token)
         : base(logger, storeReader, storeWriter, config, token)
        {
        }

        public CrmFileDataImporterCsv(ILogger logger, IEntityRepository entityRepo, IFileStoreReaderConfig readerConfig, ICrmStoreWriterConfig writerConfig, ICrmGenericImporterConfig importConfig, CrmSchemaConfiguration schemaConfig, CancellationToken token)
            : base(
                  logger,
                  new DataFileStoreReaderCsv(logger, readerConfig, schemaConfig),
                  new DataCrmStoreWriter(logger, entityRepo, writerConfig, token),
                  importConfig,
                  token)
        {
        }

        public CrmFileDataImporterCsv(ILogger logger, IEntityRepository entityRepo, CrmImportConfig importConfig, CrmSchemaConfiguration schemaConfig, CancellationToken token)
          : base(
                logger,
                new DataFileStoreReaderCsv(logger, importConfig, schemaConfig),
                new DataCrmStoreWriter(logger, entityRepo, importConfig, token),
                importConfig,
                token)
        {
        }

        public CrmFileDataImporterCsv(ILogger logger, List<IEntityRepository> entityRepos, IFileStoreReaderConfig readerConfig, ICrmStoreWriterConfig writerConfig, ICrmGenericImporterConfig importConfig, CrmSchemaConfiguration schemaConfig, CancellationToken token)
          : base(
                logger,
                new DataFileStoreReaderCsv(logger, readerConfig, schemaConfig),
                new DataCrmStoreWriterMultiThreaded(logger, entityRepos, writerConfig, token),
                importConfig,
                token)
        {
        }

        public CrmFileDataImporterCsv(ILogger logger, List<IEntityRepository> entityRepos, CrmImportConfig importConfig, CrmSchemaConfiguration schemaConfig, CancellationToken token)
         : base(
               logger,
               new DataFileStoreReaderCsv(logger, importConfig, schemaConfig),
               new DataCrmStoreWriterMultiThreaded(logger, entityRepos, importConfig, token),
               importConfig,
               token)
        {
        }
    }
}