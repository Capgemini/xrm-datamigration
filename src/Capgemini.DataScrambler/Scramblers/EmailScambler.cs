using Capgemini.DataMigration.Core.Extensions;
using Capgemini.DataMigration.Exceptions;

namespace Capgemini.DataScrambler.Scramblers
{
    public class EmailScambler : StringScrambler
    {
        public EmailScambler()
            : base()
        {
        }

        public override string Scramble(string input, int min, int max)
        {
            input.ThrowArgumentNullExceptionIfNull(nameof(input));

            string[] partsOfEmail = input.Split('@');
            ValidateEmail(partsOfEmail);

            partsOfEmail[0] = ScrambleString(partsOfEmail[0], min, max);
            partsOfEmail[1] = ScrambleString(partsOfEmail[1], min, max);
            return string.Join("@", partsOfEmail);
        }

        private static void ValidateEmail(string[] partsOfEmail)
        {
            if (partsOfEmail.Length != 2)
            {
                throw new ValidationException("EmailScambler: This is not a valid email");
            }
        }
    }
}