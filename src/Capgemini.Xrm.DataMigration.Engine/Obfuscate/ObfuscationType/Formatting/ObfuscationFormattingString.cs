using System;
using System.Collections.Generic;
using System.Globalization;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataScrambler;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting.FormattingOptions;

namespace Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting
{
    public class ObfuscationFormattingString : IObfuscationFormattingType<string>
    {
        public string CreateFormattedValue(string originalValue, FieldToBeObfuscated field, Dictionary<string, object> metadataParameters)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

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
                        obfuscatedStrings.Add(FormattingOptionProcessor.GenerateRandomString(originalValue, arg));
                        break;
                    case ObfuscationFormatType.RandomNumber:
                        obfuscatedStrings.Add(FormattingOptionProcessor.GenerateRandomNumber(originalValue, arg).ToString(CultureInfo.InvariantCulture));
                        break;
                    case ObfuscationFormatType.Lookup:
                        obfuscatedStrings.Add(FormattingOptionProcessor.GenerateFromLookup(originalValue, arg));
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
