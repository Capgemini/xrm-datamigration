using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capgemini.DataScrambler.Scramblers
{
    internal static class RandomGenerator
    {
        internal static Random GetRandom { get; } = new Random();
    }
}
