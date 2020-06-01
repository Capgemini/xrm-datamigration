using Capgemini.DataMigration.DataStore;

namespace Capgemini.DataMigration.Core
{
    /// <summary>
    /// Data Processor Interface.
    /// </summary>
    /// <typeparam name="TMigrationEntity">TMigrationEntity.</typeparam>
    /// <typeparam name="TMigrationEntityWrapper">TMigrationEntityWrapper.</typeparam>
    public interface IEntityProcessor<TMigrationEntity, TMigrationEntityWrapper>
        where TMigrationEntity : class
        where TMigrationEntityWrapper : MigrationEntityWrapper<TMigrationEntity>
    {
        /// <summary>
        /// Gets minimum number of passes through data required by the processor.
        /// </summary>
        int MinRequiredPassNumber { get; }

        /// <summary>
        /// Process Entity.
        /// </summary>
        /// <param name="entity">Entity to process.</param>
        /// <param name="passNumber">Pass Number.</param>
        /// <param name="maxPassNumber">maxPassNumber.</param>
        void ProcessEntity(TMigrationEntityWrapper entity, int passNumber, int maxPassNumber);

        /// <summary>
        /// Called by engine when import is being initialized.
        /// </summary>
        void ImportStarted();

        /// <summary>
        /// Called by engine when import is completed.
        /// </summary>
        void ImportCompleted();
    }
}