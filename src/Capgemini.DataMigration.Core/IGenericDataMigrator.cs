using Capgemini.DataMigration.DataStore;

namespace Capgemini.DataMigration.Core
{
    public interface IGenericDataMigrator<TMigrationEntity, TMigrationEntityWrapper>
        where TMigrationEntity : class
        where TMigrationEntityWrapper : MigrationEntityWrapper<TMigrationEntity>
    {
        void AddProcessor(IEntityProcessor<TMigrationEntity, TMigrationEntityWrapper> processor);
        int GetStartingPassNumber();
        void MigrateData();
    }
}