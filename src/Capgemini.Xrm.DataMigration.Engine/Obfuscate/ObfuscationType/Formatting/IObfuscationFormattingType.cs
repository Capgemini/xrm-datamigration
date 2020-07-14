using Capgemini.DataMigration.Core.Model;
using Capgemini.DataScrambler;

namespace Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting
{
    public interface IObfuscationFormattingType<T>
    {
        T CreateFormattedValue(T originalValue, FieldToBeObfuscated field);
    }
}
