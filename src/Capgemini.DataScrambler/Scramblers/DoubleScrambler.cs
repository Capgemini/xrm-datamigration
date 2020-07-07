using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capgemini.DataScrambler.Scramblers
{
    public class DoubleScrambler : IScrambler<double>
    {
        Random RandomGenerator { get; }

        public DoubleScrambler()
        {
            RandomGenerator = new Random();
        }

        public double Scramble(double input, int min, int max)
        {
            double randomNumber = CalculateRandomDouble(max);
            while (randomNumber == input && randomNumber < min)
            {
                randomNumber = CalculateRandomDouble(max);
            }

            return randomNumber;
        }

        private double CalculateRandomDouble(int max)
        {
            return RandomGenerator.NextDouble() * max;
        }
    }
}
