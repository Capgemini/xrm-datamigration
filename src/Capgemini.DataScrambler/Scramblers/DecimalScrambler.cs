namespace Capgemini.DataScrambler.Scramblers
{
    public class DecimalScrambler : IScrambler<decimal>
    {
        private readonly DoubleScrambler doubleScrambler;

        public DecimalScrambler()
        {
            doubleScrambler = new DoubleScrambler();
        }

        public decimal Scramble(decimal input, int min, int max)
        {
            double inputAsDouble = (double)input;
            return (decimal)doubleScrambler.Scramble(inputAsDouble, min, max);
        }
    }
}