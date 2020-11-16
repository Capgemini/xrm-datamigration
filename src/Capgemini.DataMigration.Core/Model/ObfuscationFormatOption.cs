using System.Collections.Generic;

namespace Capgemini.DataMigration.Core.Model
{
    public class ObfuscationFormatOption
    {
        public ObfuscationFormatOption(ObfuscationFormatType formatType, Dictionary<string, string> arguments)
        {
            FormatType = formatType;
            Arguments = arguments;
        }

        /// <summary>
        /// Gets the type of formatting to be used when obfuscating.
        /// </summary>
        public ObfuscationFormatType FormatType { get; }

        /// <summary>
        /// Gets the arguments that will be used when data is being obfuscated.
        /// </summary>
        public Dictionary<string, string> Arguments { get; }
    }
}