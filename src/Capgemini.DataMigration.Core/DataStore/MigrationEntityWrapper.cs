using System;
using Capgemini.DataMigration.Core;

namespace Capgemini.DataMigration.DataStore
{
    public abstract class MigrationEntityWrapper<TMigrationEntity>
        where TMigrationEntity : class
    {
        protected MigrationEntityWrapper(TMigrationEntity entity, bool isManyToMany = false)
        {
            OriginalEntity = entity;
            OperationType = OperationType.New;
            IsManyToMany = isManyToMany;
        }

        public OperationType OperationType { get; set; }

        public TMigrationEntity OriginalEntity { get; private set; }

        public string OperationResult { get; set; }

        public int? OperationErrorCode { get; set; }

        public bool IsManyToMany { get; internal set; }

        public abstract Guid Id { get; }

        public abstract string LogicalName { get; }

        public abstract int AttributesCount { get; }
    }
}