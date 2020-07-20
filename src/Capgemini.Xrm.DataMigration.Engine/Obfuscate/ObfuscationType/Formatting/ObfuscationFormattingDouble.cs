using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.Xml;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataScrambler;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting.FormattingOptions;

namespace Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting
{
    public class ObfuscationFormattingDouble : IObfuscationFormattingType<double>
    {
        public double CreateFormattedValue(double originalValue, FieldToBeObfuscated field, Dictionary<string, object> metadataParameters)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            var replacementValue = Format(originalValue, field);

            while (replacementValue == originalValue || !ReplacementIsValid(replacementValue, metadataParameters))
            {
                replacementValue = Format(originalValue, field);
            }

            return replacementValue;
        }

        private bool ReplacementIsValid(double replacementValue, Dictionary<string, object> metadataParameters)
        {
            int min = 0;
            int max = 10;

            if (metadataParameters != null && metadataParameters.ContainsKey("min"))
            {
                if (int.TryParse(metadataParameters["min"].ToString(), out int value))
                {
                    min = value;
                }
            }

            if (metadataParameters != null && metadataParameters.ContainsKey("max"))
            {
                if (int.TryParse(metadataParameters["max"].ToString(), out int value))
                {
                    max = value;
                }
            }

            if (replacementValue < min || replacementValue > max)
            {
                return false;
            }

            return true;
        }

        private double Format(double originalValue, FieldToBeObfuscated field)
        {
            List<string> obfuscatedStrings = new List<string>();

            if (field.ObfuscationFormatArgs.Count > 1)
            {
                throw new Exception($"Only a single ObfuscationFormatOption of type {ObfuscationFormatType.Lookup} can be applied to a field of type double.");
            }

            if (!field.ObfuscationFormatArgs[0].FormatType.Equals(ObfuscationFormatType.Lookup))
            {
                throw new NotImplementedException($"The ObfuscationFormatType({field.ObfuscationFormatArgs[0].FormatType}) is not implemented for fields of type double.");
            }

            string lookupValue = FormattingOptionProcessor.GenerateFromLookup(originalValue.ToString("G", CultureInfo.InvariantCulture), field.ObfuscationFormatArgs[0]);

            if (double.TryParse(lookupValue, out double replacementValue))
            {
                return replacementValue;
            }

            throw new InvalidCastException($"The value ({lookupValue}) read from the lookup source ({field.ObfuscationFormatArgs[0].Arguments["filename"]}) field ({field.ObfuscationFormatArgs[0].Arguments["columnname"]}) could not be converted to a double.");
        }
    }
}
