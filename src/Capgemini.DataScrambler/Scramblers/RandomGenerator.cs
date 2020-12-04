using System;

namespace Capgemini.DataScrambler.Scramblers
{
    internal static class RandomGenerator
    {
        internal static Random GetRandom { get; } = new Random();
    }
}