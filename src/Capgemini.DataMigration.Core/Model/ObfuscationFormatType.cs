using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Capgemini.DataMigration.Core.Model
{
    /// <summary>
    /// The formatting type used by a format option.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ObfuscationFormatType
    {
        /// <summary>
        /// Generates a random string value
        /// </summary>
        RandomString,

        /// <summary>
        /// Generates a random number.
        /// </summary>
        RandomNumber,

        /// <summary>
        /// Looks up the data from an external source.
        /// </summary>
        Lookup
    }
}
