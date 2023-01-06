using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.DataMigration.DataStore;

namespace Capgemini.DataMigration.Core
{
    /// <summary>
    /// Generic Data Migrator Engine.
    /// </summary>
    public class GenericDataMigrator<TMigrationEntity, TMigrationEntityWrapper> : IGenericDataMigrator<TMigrationEntity, TMigrationEntityWrapper>
        where TMigrationEntity : class
        where TMigrationEntityWrapper : MigrationEntityWrapper<TMigrationEntity>
    {
        private readonly IDataStoreWriter<TMigrationEntity, TMigrationEntityWrapper> storeWriter;
        private readonly IDataStoreReader<TMigrationEntity, TMigrationEntityWrapper> storeReader;
        private readonly CancellationToken cancellationToken;
        private readonly List<IEntityProcessor<TMigrationEntity, TMigrationEntityWrapper>> processors;
        private int maxPassNo = 1;

        public GenericDataMigrator(ILogger logger, IDataStoreReader<TMigrationEntity, TMigrationEntityWrapper> storeReader, IDataStoreWriter<TMigrationEntity, TMigrationEntityWrapper> storeWriter)
        {
            logger.ThrowIfNull<ArgumentNullException>(nameof(logger));
            storeReader.ThrowIfNull<ArgumentNullException>(nameof(storeReader));
            storeWriter.ThrowIfNull<ArgumentNullException>(nameof(storeWriter));

            this.storeReader = storeReader;
            this.storeWriter = storeWriter;
            this.Logger = logger;

            cancellationToken = new CancellationTokenSource().Token;
            processors = new List<IEntityProcessor<TMigrationEntity, TMigrationEntityWrapper>>();
        }

        public GenericDataMigrator(ILogger logger, IDataStoreReader<TMigrationEntity, TMigrationEntityWrapper> storeReader, IDataStoreWriter<TMigrationEntity, TMigrationEntityWrapper> storeWriter, CancellationToken cancellationToken)
            : this(logger, storeReader, storeWriter)
        {
            this.cancellationToken = cancellationToken;
        }

        protected IDataStoreReader<TMigrationEntity, TMigrationEntityWrapper> GetStoreReader => storeReader;

        protected ILogger Logger { get; }

        public void AddProcessor(IEntityProcessor<TMigrationEntity, TMigrationEntityWrapper> processor)
        {
            processors.Add(processor);
            Logger.LogVerbose($"Using {processor?.GetType().Name} processor");
        }

        public void MigrateData()
        {
            Logger.LogInfo("GenericDataMigrator MigrateData started");

            var processorList = GetProcessors();

            if (processorList != null && processorList.Count > 0)
            {
                maxPassNo = processorList.Select(p => p.MinRequiredPassNumber).Max();

                Logger.LogInfo($"GenericDataMigrator MigrateData Starting Import, processorsCount:{processorList.Count}, maxPassNo{maxPassNo}");
                processorList.ForEach(p => p.ImportStarted());
            }

            int passNo = GetStartingPassNumber();

            while (passNo <= maxPassNo)
            {
                PerformMigratePass(passNo++, processorList);
            }

            if (processorList != null && processorList.Count > 0)
            {
                Logger.LogInfo($"GenericDataMigrator MigrateData Completing Import, processorsCount:{processorList.Count}");
                processorList.ForEach(p => p.ImportCompleted());
            }

            Logger.LogInfo("GenericDataMigrator MigrateData finished");
        }

        public virtual int GetStartingPassNumber()
        {
            return 1;
        }

        protected virtual void PerformMigratePass(int passNo, List<IEntityProcessor<TMigrationEntity, TMigrationEntityWrapper>> processors)
        {
            Logger.LogInfo($"GenericDataMigrator PerformMigratePass started, passNo:{passNo}");

            storeWriter.Reset();
            storeReader.Reset();
            int batchNo = 1;

            List<TMigrationEntityWrapper> data = storeReader.ReadBatchDataFromStore();

            var logMessage = new StringBuilder();
            logMessage.Append($"GenericDataMigrator PerformMigratePass retrieved data, batchNo:{batchNo} entities:{data.Count} ");
            if (data.Count > 0)
            {
                logMessage.Append($" FirstEntity:{data.First().LogicalName}");
            }

            Logger.LogInfo(logMessage.ToString());

            while (data.Count > 0)
            {
                if (processors != null && processors.Count > 0)
                {
                    ProcessData(data, passNo, processors);
                }

                cancellationToken.ThrowIfCancellationRequested();
                storeWriter.SaveBatchDataToStore(data);
                Logger.LogInfo($"GenericDataMigrator PerformMigratePass saved data, batchNo:{batchNo} entities:{data.Count}");

                cancellationToken.ThrowIfCancellationRequested();

                batchNo++;
                data = storeReader.ReadBatchDataFromStore();

                var message = $"GenericDataMigrator PerformMigratePass retrieved data, batchNo:{batchNo} entities:{data.Count} {data.FirstOrDefault()?.LogicalName}";

                Logger.LogInfo(message);
            }

            Logger.LogInfo($"GenericDataMigrator PerformMigratePass finished, passNo:{passNo}");
        }

        protected virtual List<IEntityProcessor<TMigrationEntity, TMigrationEntityWrapper>> GetProcessors()
        {
            return processors;
        }

        protected virtual void ProcessData(List<TMigrationEntityWrapper> entities, int passNo, List<IEntityProcessor<TMigrationEntity, TMigrationEntityWrapper>> processors)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Logger.LogVerbose($"GenericDataMigrator ProcessData started, passNo:{passNo} entities:{entities?.Count}");

            if (processors != null)
            {
                foreach (var entity in entities)
                {
                    processors.ForEach(p => p.ProcessEntity(entity, passNo, maxPassNo));
                }
            }

            var entToRemove = entities.Where(p => p.OperationType == OperationType.Ignore || p.AttributesCount == 0).ToList();
            entToRemove.ForEach(p =>
            {
                entities.Remove(p);
                Logger.LogVerbose($"Entity: {p.LogicalName} Id: {p.Id} has been ignored: {(p.OperationType == OperationType.Ignore ? "Processors" : "No attributes")}");
            });

            Logger.LogVerbose($"GenericDataMigrator ProcessData finished, passNo:{passNo} entities:{entities.Count}");
        }
    }
}