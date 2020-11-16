using System;
using System.Collections.Generic;
using System.Globalization;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataMigration.Exceptions;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting.FormattingOptions;

namespace Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting
{
    public class ObfuscationFormattingDouble : IObfuscationFormattingType<double>
    {
        private readonly FormattingOptionProcessor optionProcessor;

        public ObfuscationFormattingDouble(FormattingOptionProcessor processor)
        {
            optionProcessor = processor;
        }

        public static bool ReplacementIsValid(double replacementValue, Dictionary<string, object> metadataParameters)
        {
            int min = 0;
            int max = 10;
            var result = true;

            if (metadataParameters != null && metadataParameters.ContainsKey("min") && int.TryParse(metadataParameters["min"].ToString(), out int minValue))
            {
                min = minValue;
            }

            if (metadataParameters != null && metadataParameters.ContainsKey("max") && int.TryParse(metadataParameters["max"].ToString(), out int maxValue))
            {
                max = maxValue;
            }

            if (replacementValue < min || replacementValue > max)
            {
                result = false;
            }

            return result;
        }

        public double CreateFormattedValue(double originalValue, FieldToBeObfuscated field, Dictionary<string, object> metadataParameters)
        {
            metadataParameters.ThrowArgumentNullExceptionIfNull(nameof(metadataParameters));
            field.ThrowArgumentNullExceptionIfNull(nameof(field));

            var replacementValue = Format(originalValue, field);

            while (replacementValue == originalValue || !ReplacementIsValid(replacementValue, metadataParameters))
            {
                replacementValue = Format(originalValue, field);
            }

            return replacementValue;
        }

        private double Format(double originalValue, FieldToBeObfuscated field)
        {
            if (field.ObfuscationFormatArgs.Count > 1)
            {
                throw new ValidationException($"Only a single ObfuscationFormatOption of type {ObfuscationFormatType.Lookup} can be applied to a field of type double.");
            }

            if (!field.ObfuscationFormatArgs[0].FormatType.Equals(ObfuscationFormatType.Lookup))
            {
                throw new NotImplementedException($"The ObfuscationFormatType({field.ObfuscationFormatArgs[0].FormatType}) is not implemented for fields of type double.");
            }

            string lookupValue = optionProcessor.GenerateFromLookup(originalValue.ToString("G", CultureInfo.InvariantCulture), field.ObfuscationFormatArgs[0]);

            if (double.TryParse(lookupValue, out double replacementValue))
            {
                return replacementValue;
            }

            throw new InvalidCastException($"The value ({lookupValue}) read from the lookup source ({field.ObfuscationFormatArgs[0].Arguments["filename"]}) field ({field.ObfuscationFormatArgs[0].Arguments["columnname"]}) could not be converted to a double.");
        }
    }
}