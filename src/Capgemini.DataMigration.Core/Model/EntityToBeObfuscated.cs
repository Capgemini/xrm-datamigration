using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Contains a list of the fields that will be obfuscated.
        /// </summary>
        public List<FieldToBeObfuscated> FieldsToBeObfuscated { get; set; }

    }
}
