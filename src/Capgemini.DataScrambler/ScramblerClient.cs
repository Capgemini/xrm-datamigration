using Capgemini.DataScrambler.Scramblers;

namespace Capgemini.DataScrambler
{
    public class ScramblerClient<T>
    {
        private IScrambler<T> currentScrambler;

        public ScramblerClient(IScrambler<T> scrambler)
        {
            currentScrambler = scrambler;
        }

        public T ExecuteScramble(T input, int min = 0, int max = 10)
        {
            return currentScrambler.Scramble(input, min, max);
        }

        public void ChangeScrambler(IScrambler<T> newScrambler)
        {
            currentScrambler = newScrambler;
        }
    }
}