using System;
using Capgemini.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Model;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.DataStore
{
    public class EntityWrapper : MigrationEntityWrapper<Entity>
    {
        public EntityWrapper(Entity entity, bool isManyToMany = false)
            : base(entity, isManyToMany)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            StateCode = -1;
            StatusCode = -1;

            if (entity.Attributes.Contains(EntityFields.StateCode))
            {
                StateCode = ((OptionSetValue)entity[EntityFields.StateCode]).Value;
            }

            if (entity.Attributes.Contains(EntityFields.StatusCode))
            {
                StatusCode = ((OptionSetValue)entity[EntityFields.StatusCode]).Value;
            }

            if (entity.Attributes.Contains(EntityFields.OwnerId))
            {
                OwnerId = (EntityReference)entity[EntityFields.OwnerId];
            }
        }

        public int StateCode { get; private set; }

        public int StatusCode { get; private set; }

        public EntityReference OwnerId { get; private set; }

        public override Guid Id
        {
            get
            {
                return this.OriginalEntity.Id;
            }
        }

        public override string LogicalName
        {
            get
            {
                return this.OriginalEntity.LogicalName;
            }
        }

        public override int AttributesCount
        {
            get
            {
                return this.OriginalEntity.Attributes.Count;
            }
        }
    }
}