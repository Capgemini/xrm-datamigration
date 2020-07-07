using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capgemini.DataScrambler.Scramblers
{
    public class EmailScambler : StringScrambler
    {
        public EmailScambler() : base()
        {

        }

        public override string Scramble(string input, int min, int max)
        {
            string[] partsOfEmail = input.Split('@');
            ValidateEmail(partsOfEmail);

            partsOfEmail[0] = ScrambleString(partsOfEmail[0], min, max);
            partsOfEmail[1] = ScrambleString(partsOfEmail[1], min, max);
            return String.Join("@", partsOfEmail);
        }

        private void ValidateEmail(string[] partsOfEmail)
        {
            if (partsOfEmail.Length != 2)
            {
                throw new Exception("EmailScambler: This is not a valid email");
            }
        }
    }
}
