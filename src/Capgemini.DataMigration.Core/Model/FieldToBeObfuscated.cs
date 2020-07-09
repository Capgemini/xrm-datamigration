using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capgemini.DataMigration.Core.Model
{
    /// <summary>
    /// Contains details of the field to be obfuscated
    /// </summary>
    public class FieldToBeObfuscated
    {
        /// <summary>
        /// Gets or Sets the field name.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or Sets the formatting to be used when obfuscating the data.
        /// </summary>
        public string ObfuscationFormat { get; set; }

        /// <summary>
        /// The functions that will be used to generate the obfuscated values
        /// </summary>
        public List<string> ObfuscationFormatArgs { get; set; }

    }
}
