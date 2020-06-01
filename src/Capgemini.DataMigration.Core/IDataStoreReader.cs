using System.Collections.Generic;
using Capgemini.DataMigration.DataStore;

namespace Capgemini.DataMigration.Core
{
    public interface IDataStoreReader<TMigrationEntity, TMigrationEntityWrapper>
        where TMigrationEntity : class
        where TMigrationEntityWrapper : MigrationEntityWrapper<TMigrationEntity>
    {
        List<TMigrationEntityWrapper> ReadBatchDataFromStore();

        void Reset();
    }
}