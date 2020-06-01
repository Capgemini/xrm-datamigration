using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.CrmStore.DataStores;
using Capgemini.Xrm.DataMigration.DataStore;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.Implementation
{
    [ExcludeFromCodeCoverage]
    public class TestCrmGenericImporter : CrmGenericImporter
    {
        public TestCrmGenericImporter(
            ILogger logger,
            IDataStoreReader<Entity, EntityWrapper> storeReader,
            DataCrmStoreWriter storeWriter,
            ICrmGenericImporterConfig config)
            : base(logger, storeReader, storeWriter, config)
        {
        }
    }
}