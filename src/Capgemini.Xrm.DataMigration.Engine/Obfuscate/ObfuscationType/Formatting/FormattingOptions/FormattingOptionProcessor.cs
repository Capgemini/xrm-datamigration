using System;
using System.Globalization;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataMigration.Exceptions;
using Capgemini.DataScrambler;
using Capgemini.DataScrambler.Scramblers;

namespace Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting.FormattingOptions
{
    public class FormattingOptionProcessor
    {
        private readonly FormattingOptionLookup formattingOptionLookup;

        public FormattingOptionProcessor()
        {
            this.formattingOptionLookup = new FormattingOptionLookup();
        }

        public FormattingOptionProcessor(FormattingOptionLookup formattingOptionLookup)
        {
            this.formattingOptionLookup = formattingOptionLookup;
        }

        public static int GenerateRandomNumber(string originalValue, ObfuscationFormatOption arg)
        {
            originalValue.ThrowArgumentNullExceptionIfNull(nameof(originalValue));
            arg.ThrowArgumentNullExceptionIfNull(nameof(arg));

            int min = 0;
            int max = 10;

            if (arg.Arguments.ContainsKey("min") && int.TryParse(arg.Arguments["min"], out int minResult))
            {
                min = minResult;
            }

            if (arg.Arguments.ContainsKey("max") && int.TryParse(arg.Arguments["max"], out int maxResult))
            {
                max = maxResult;
            }

            if (min >= max)
            {
                throw new ArgumentOutOfRangeException($"The min({min}) must be lower than the max({max}).");
            }

            int randomNumber = RandomGenerator.Next(min, max);
            while (originalValue.Contains(randomNumber.ToString(CultureInfo.InvariantCulture)))
            {
                randomNumber = RandomGenerator.Next(min, max);
            }

            return randomNumber;
        }

        public virtual string GenerateFromLookup(string originalValue, ObfuscationFormatOption arg)
        {
            originalValue.ThrowArgumentNullExceptionIfNull(nameof(originalValue));
            arg.ThrowArgumentNullExceptionIfNull(nameof(arg));

            string fileName = string.Empty;

            if (arg.Arguments.ContainsKey("filename"))
            {
                fileName = arg.Arguments["filename"];
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ValidationException($"Obfuscation format option {nameof(fileName)} should not be null");
            }

            string columnName = string.Empty;

            if (arg.Arguments.ContainsKey("columnname"))
            {
                columnName = arg.Arguments["columnname"];
            }

            if (string.IsNullOrEmpty(columnName))
            {
                throw new ValidationException($"Obfuscation format option {nameof(columnName)} should not be null");
            }

            ObfuscationLookup obfuscationLookup = formattingOptionLookup.GetObfuscationLookup(fileName);

            var newValue = (string)LookupRandomValue(columnName, obfuscationLookup);

            while (originalValue.Contains(newValue))
            {
                newValue = (string)LookupRandomValue(columnName, obfuscationLookup);
            }

            return newValue;
        }

        public virtual string GenerateRandomString(string originalValue, ObfuscationFormatOption arg)
        {
            originalValue.ThrowArgumentNullExceptionIfNull(nameof(originalValue));
            arg.ThrowArgumentNullExceptionIfNull(nameof(arg));

            var scramblerClient = new ScramblerClient<string>(new StringScrambler());
            int length = 5;

            if (arg.Arguments.ContainsKey("length"))
            {
                if (int.TryParse(arg.Arguments["length"], out int result))
                {
                    length = result;
                }

                if (originalValue.Length > length)
                {
                    return scramblerClient.ExecuteScramble(originalValue.Substring(0, length));
                }

                return scramblerClient.ExecuteScramble(originalValue.PadRight(length, 'A'));
            }

            return scramblerClient.ExecuteScramble(originalValue);
        }

        private object LookupRandomValue(string columnName, ObfuscationLookup lookup)
        {
            int index = RandomGenerator.Next(lookup.Count - 1);

            var newValue = lookup[index, columnName];
            return newValue;
        }
    }
}