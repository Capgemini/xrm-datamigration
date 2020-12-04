using System.Collections.Generic;

namespace Capgemini.DataMigration.Core.Model
{
    /// <summary>
    /// Stores details of an Entity that contains fields that will be obfuscated.
    /// </summary>
    public class EntityToBeObfuscated
    {
        /// <summary>
        /// Gets or Sets the name of the Entity.
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Gets the fields that will be obfuscated.
        /// </summary>
        public List<FieldToBeObfuscated> FieldsToBeObfuscated { get; private set; } = new List<FieldToBeObfuscated>();
    }
}