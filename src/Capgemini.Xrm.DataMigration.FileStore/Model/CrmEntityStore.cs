using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.Xrm.DataMigration.DataStore;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.FileStore.Model
{
    /// <summary>
    /// A class to store CRM entity.
    /// </summary>
    [Serializable]
    public class CrmEntityStore
    {
        public CrmEntityStore()
        {
            Attributes = new List<CrmAttributeStore>();
        }

        public CrmEntityStore(Entity entity)
            : this()
        {
            entity.ThrowArgumentNullExceptionIfNull(nameof(entity));

            Id = entity.Id;
            LogicalName = entity.LogicalName;

            Attributes = entity.Attributes
                .Where(a => a.Key.ToUpper(CultureInfo.InvariantCulture) != $"{LogicalName.ToUpper(CultureInfo.InvariantCulture)}ID" && a.Key.ToUpper(CultureInfo.InvariantCulture) != "LOGICALNAME")
                .Select(a => new CrmAttributeStore(a)).ToList();
        }

        public CrmEntityStore(EntityWrapper entity)
            : this()
        {
            entity.ThrowArgumentNullExceptionIfNull(nameof(entity));

            Id = entity.OriginalEntity.Id;
            LogicalName = entity.OriginalEntity.LogicalName;
            IsManyToMany = entity.IsManyToMany;
            Attributes = entity.OriginalEntity.Attributes
                .Where(a => a.Key.ToUpper(CultureInfo.InvariantCulture) != $"{LogicalName.ToUpper(CultureInfo.InvariantCulture)}ID" && a.Key.ToUpper(CultureInfo.InvariantCulture) != "LOGICALNAME")
                .Select(a => new CrmAttributeStore(a)).ToList();
        }

        public string LogicalName { get; set; }

        public Guid Id { get; set; }

        public bool IsManyToMany { get; set; }

        public List<CrmAttributeStore> Attributes { get; }
    }
}