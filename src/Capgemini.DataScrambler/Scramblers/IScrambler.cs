using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capgemini.DataScrambler.Scramblers
{
    public interface IScrambler<T>
    {
        T Scramble(T input, int min, int max);
    }

}
