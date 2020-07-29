using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capgemini.DataScrambler.Scramblers
{
    public class IntegerScrambler : IScrambler<int>
    {        
        public IntegerScrambler()
        {
        }

        public int Scramble(int input, int min, int max)
        {

            int randomNumber = RandomGenerator.GetRandom.Next(min, max);
            while(randomNumber == input)
            {
                randomNumber = RandomGenerator.GetRandom.Next(min, max);
            }

            return randomNumber;
        }
    }
}
