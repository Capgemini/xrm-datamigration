using System.Collections.Generic;
using Capgemini.DataMigration.DataStore;

namespace Capgemini.DataMigration.Core
{
    public interface IDataStoreWriter<TMigrationEntity, TMigrationEntityWrapper>
        where TMigrationEntity : class
        where TMigrationEntityWrapper : MigrationEntityWrapper<TMigrationEntity>
    {
        void SaveBatchDataToStore(List<TMigrationEntityWrapper> entities);

        void Reset();
    }
}