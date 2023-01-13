namespace Capgemini.DataScrambler.Scramblers
{
    public class DoubleScrambler : IScrambler<double>
    {
        public DoubleScrambler()
        {
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

        private static double CalculateRandomDouble(int max) => RandomGenerator.NextDouble() * max;
    }
}