using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capgemini.DataScrambler.Scramblers
{
    public class StringScrambler : IScrambler<string>
    {
        const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public StringScrambler()
        {
        }

        public virtual string Scramble(string input, int min, int max)
        {
            return ScrambleString(input, min, max);
        }

        protected string ScrambleString(string input, int min, int max)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                int randomNumber = RandomGenerator.GetRandom.Next(0, Chars.Length);
                char newCharacter = Chars[randomNumber];
                sb.Append(newCharacter);
            }

            string outputString = sb.ToString();
            if (input == outputString)
            {
                outputString = Scramble(input, min, max);
            }

            return outputString;
        }
    }
}
