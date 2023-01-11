using System;
using System.Security.Cryptography;

namespace Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting.FormattingOptions
{
    internal static class FormattingOptionProcessorStatics
    {
        internal static RandomNumberGenerator RandomGenerator { get; } = RandomNumberGenerator.Create();

        internal static double NextDouble()
        {
            var bytes = new byte[sizeof(ulong)];
            RandomGenerator.GetBytes(bytes);
            ulong nextULong = BitConverter.ToUInt64(bytes, 0);

            return (nextULong >> 11) * (1.0 / (1ul << 53));
        }

        internal static int Next(int minValue, int maxValue)
        {
            if (minValue == maxValue)
            {
                return minValue;
            }

            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue), $"{nameof(minValue)} must be lower than {nameof(maxValue)}");
            }

            var diff = (long)maxValue - minValue;

            return (int)(minValue + (diff * NextDouble()));
        }

        internal static int Next(int maxValue)
        {
            return Next(0, maxValue);
        }
    }
}