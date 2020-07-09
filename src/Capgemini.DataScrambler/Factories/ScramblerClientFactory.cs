using Capgemini.DataScrambler.Scramblers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capgemini.DataScrambler.Factories
{
    public class ScramblerClientFactory
    {
        public ScramblerClient<T> GetScrambler<T>()
        {
            Type type = typeof(T);
            if (type == typeof(int))
            {
                return new ScramblerClient<T>((IScrambler<T>)new IntegerScrambler());
            }
            else if (type == typeof(string))
            {
                return new ScramblerClient<T>((IScrambler<T>)new StringScrambler());

            }
            else if (type == typeof(Guid))
            {
                return new ScramblerClient<T>((IScrambler<T>)new GuidScrambler());
            }
            else if (type == typeof(double))
            {
                return new ScramblerClient<T>((IScrambler<T>)new DoubleScrambler());
            }
            else if (type == typeof(decimal))
            {
                return new ScramblerClient<T>((IScrambler<T>)new DecimalScrambler());
            }
            else
            {
                throw new Exception("The specified generic type could not be found");
            }

        }
    }
}
