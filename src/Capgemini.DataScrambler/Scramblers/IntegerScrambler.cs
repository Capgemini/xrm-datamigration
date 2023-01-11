namespace Capgemini.DataScrambler.Scramblers
{
    public class IntegerScrambler : IScrambler<int>
    {
        public IntegerScrambler()
        {
        }

        public int Scramble(int input, int min, int max)
        {
            int randomNumber = RandomGenerator.Next(min, max);

            while (randomNumber == input)
            {
                randomNumber = RandomGenerator.Next(min, max);
            }

            return randomNumber;
        }
    }
}