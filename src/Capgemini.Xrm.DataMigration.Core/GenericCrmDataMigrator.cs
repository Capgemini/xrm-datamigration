using System.Threading;
using Capgemini.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Core
{
    public class GenericCrmDataMigrator : GenericDataMigrator<Entity, EntityWrapper>, IGenericCrmDataMigrator
    {
        public GenericCrmDataMigrator(ILogger logger, IDataStoreReader<Entity, EntityWrapper> storeReader, IDataStoreWriter<Entity, EntityWrapper> storeWriter)
            : base(logger, storeReader, storeWriter)
        {
        }

        public GenericCrmDataMigrator(ILogger logger, IDataStoreReader<Entity, EntityWrapper> storeReader, IDataStoreWriter<Entity, EntityWrapper> storeWriter, CancellationToken cancellationToken)
            : base(logger, storeReader, storeWriter, cancellationToken)
        {
        }
    }
}