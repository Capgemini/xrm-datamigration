using System;
using Capgemini.DataScrambler.Scramblers;

namespace Capgemini.DataScrambler.Factories
{
    public static class ScramblerClientFactory
    {
        public static ScramblerClient<T> GetScrambler<T>()
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
                throw new NotSupportedException($"The specified generic type {type.Name} could not be found");
            }
        }
    }
}