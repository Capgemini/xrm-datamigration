namespace Capgemini.DataScrambler.Scramblers
{
    public class IntegerScrambler : IScrambler<int>
    {
        public IntegerScrambler()
        {
        }

        public int Scramble(int input, int min, int max)
        {
            var getRandom = RandomGenerator.GetRandom;
            int randomNumber = getRandom.Next(min, max);

            while (randomNumber == input)
            {
                randomNumber = getRandom.Next(min, max);
            }

            return randomNumber;
        }
    }
}