using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Capgemini.DataMigration.Core.Model
{
    /// <summary>
    /// Type of field being obfuscated.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FieldDataType
    {
        /// <summary>
        /// Uses the data type of the Dynamics fields
        /// </summary>
        Default,

        /// <summary>
        /// Emailaddress
        /// </summary>
        EmailAddress
    }
}
