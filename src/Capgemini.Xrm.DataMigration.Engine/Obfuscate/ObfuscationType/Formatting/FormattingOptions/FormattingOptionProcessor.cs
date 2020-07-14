using System;
using System.Globalization;
using Capgemini.DataMigration.Core.Helpers;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataScrambler;
using Capgemini.DataScrambler.Scramblers;

namespace Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting.FormattingOptions
{
    public static class FormattingOptionProcessor
    {
        private static Random randomGenerator = new Random();

        public static int GenerateRandomNumber(string originalValue, ObfuscationFormatOption arg)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(nameof(arg));
            }

            int min = 0;
            int max = 10;

            if (arg.Arguments.ContainsKey("min"))
            {
                if (int.TryParse(arg.Arguments["min"], out int result))
                {
                    min = result;
                }
            }

            if (arg.Arguments.ContainsKey("max"))
            {
                if (int.TryParse(arg.Arguments["max"], out int result))
                {
                    max = result;
                }
            }

            int randomNumber = randomGenerator.Next(min, max);
            while (originalValue.Contains(randomNumber.ToString(CultureInfo.InvariantCulture)))
            {
                randomNumber = randomGenerator.Next(min, max);
            }

            return randomNumber;
        }

        public static string GenerateFromLookup(string originalValue, ObfuscationFormatOption arg)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(nameof(arg));
            }

            string fileName = string.Empty;

            if (arg.Arguments.ContainsKey("filename"))
            {
                fileName = arg.Arguments["filename"];
            }

            string columnName = string.Empty;

            if (arg.Arguments.ContainsKey("columnname"))
            {
                columnName = arg.Arguments["columnname"];
            }

            var obfuscationLookup = ObfuscationLookupHelper.ObfuscationLookups[fileName];

            var newValue = (string)LookupRandomValue(columnName, obfuscationLookup);

            while (originalValue.Contains(newValue))
            {
                newValue = (string)LookupRandomValue(columnName, obfuscationLookup);
            }

            return newValue;
        }

        public static string GenerateRandomString(string originalValue, ObfuscationFormatOption arg)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(nameof(arg));
            }

            if (originalValue == null)
            {
                throw new ArgumentNullException(nameof(originalValue));
            }

            var scramblerClient = new ScramblerClient<string>(new StringScrambler());
            if (scramblerClient == null)
            {
                throw new NullReferenceException(nameof(scramblerClient));
            }

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

        private static object LookupRandomValue(string columnName, ObfuscationLookup lookup)
        {
            int index = randomGenerator.Next(lookup.Count - 1);

            var newValue = lookup[index, columnName];
            return newValue;
        }
    }
}
