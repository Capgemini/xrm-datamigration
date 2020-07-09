using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capgemini.DataScrambler.Scramblers
{
    public class IntegerScrambler : IScrambler<int>
    {
        Random RandomGenerator { get; }
        
        public IntegerScrambler()
        {
            RandomGenerator = new Random();
        }

        public int Scramble(int input, int min, int max)
        {

            int randomNumber = RandomGenerator.Next(min, max);
            while(randomNumber == input)
            {
                randomNumber = RandomGenerator.Next(min, max);
            }

            return randomNumber;
        }
    }
}
