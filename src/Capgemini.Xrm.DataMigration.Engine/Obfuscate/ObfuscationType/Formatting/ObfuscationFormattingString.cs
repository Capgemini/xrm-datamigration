using System;
using System.Collections.Generic;
using System.Globalization;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataScrambler;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting.FormattingOptions;

namespace Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting
{
    public class ObfuscationFormattingString : IObfuscationFormattingType<string>
    {
        private FormattingOptionProcessor optionProcessor;

        public ObfuscationFormattingString(FormattingOptionProcessor processor)
        {
            optionProcessor = processor;
        }

        public string CreateFormattedValue(string originalValue, FieldToBeObfuscated field, Dictionary<string, object> metadataParameters)
        {
            field.ThrowArgumentNullExceptionIfNull(nameof(field));
            metadataParameters.ThrowArgumentNullExceptionIfNull(nameof(metadataParameters));

            string replacementString = Format(originalValue, field, metadataParameters);

            while (replacementString == originalValue)
            {
                replacementString = Format(originalValue, field, metadataParameters);
            }

            return replacementString;
        }

        private string Format(string originalValue, FieldToBeObfuscated field, Dictionary<string, object> metadataParameters)
        {
            List<string> obfuscatedStrings = new List<string>();

            foreach (var arg in field.ObfuscationFormatArgs)
            {
                switch (arg.FormatType)
                {
                    case ObfuscationFormatType.RandomString:
                        obfuscatedStrings.Add(optionProcessor.GenerateRandomString(originalValue, arg));
                        break;
                    case ObfuscationFormatType.RandomNumber:
                        obfuscatedStrings.Add(optionProcessor.GenerateRandomNumber(originalValue, arg).ToString(CultureInfo.InvariantCulture));
                        break;
                    case ObfuscationFormatType.Lookup:
                        obfuscatedStrings.Add(optionProcessor.GenerateFromLookup(originalValue, arg));
                        break;
                }
            }

            string replacementString = string.Format(CultureInfo.InvariantCulture, field.ObfuscationFormat, obfuscatedStrings.ToArray());

            if (metadataParameters != null && metadataParameters.ContainsKey("maxlength"))
            {
                var maxLength = (int)metadataParameters["maxlength"];

                if (replacementString.Length > maxLength)
                {
                    replacementString = replacementString.Substring(0, maxLength);
                }
            }

            return replacementString;
        }
    }
}
