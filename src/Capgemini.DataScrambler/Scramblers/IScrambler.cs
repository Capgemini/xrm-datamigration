namespace Capgemini.DataScrambler.Scramblers
{
    public interface IScrambler<T>
    {
        T Scramble(T input, int min, int max);
    }
}