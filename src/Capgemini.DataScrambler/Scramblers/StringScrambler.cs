using System.Text;
using Capgemini.DataMigration.Core.Extensions;

namespace Capgemini.DataScrambler.Scramblers
{
    public class StringScrambler : IScrambler<string>
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public virtual string Scramble(string input, int min, int max)
        {
            return ScrambleString(input, min, max);
        }

        protected string ScrambleString(string input, int min, int max)
        {
            input.ThrowArgumentNullExceptionIfNull(nameof(input));

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