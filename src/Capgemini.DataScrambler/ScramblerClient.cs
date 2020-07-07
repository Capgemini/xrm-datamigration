using Capgemini.DataScrambler.Scramblers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capgemini.DataScrambler
{
    public class ScramblerClient<T>
    {

        private IScrambler<T> CurrentScrambler;

        public ScramblerClient(IScrambler<T> scrambler)
        {
            CurrentScrambler = scrambler;
        }

        public T ExecuteScramble(T input, int min = 0, int max = 10)
        {
            return CurrentScrambler.Scramble(input, min, max);
        }

        public void ChangeScrambler(IScrambler<T> newScrambler)
        {
            CurrentScrambler = newScrambler;
        }
    }
}
