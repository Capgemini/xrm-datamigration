using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

[assembly: InternalsVisibleTo("Capgemini.Xrm.DataMigration.Engine")]

namespace Capgemini.DataScrambler.Scramblers
{
    internal static class RandomGenerator
    {
        private static RandomNumberGenerator GetRandom { get; } = RandomNumberGenerator.Create();

        internal static double NextDouble()
        {
            var bytes = new byte[sizeof(ulong)];
            GetRandom.GetBytes(bytes);
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