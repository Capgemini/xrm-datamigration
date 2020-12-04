using System;

namespace Capgemini.DataScrambler.Scramblers
{
    public class GuidScrambler : IScrambler<Guid>
    {
        public Guid Scramble(Guid input, int min, int max)
        {
            Guid output = Guid.NewGuid();
            if (input.Equals(output))
            {
                output = Scramble(input, min, max);
            }

            return output;
        }
    }
}