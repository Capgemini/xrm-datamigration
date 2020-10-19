using System.Collections.Generic;
using Capgemini.DataMigration.Core.Model;

namespace Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting
{
    public interface IObfuscationFormattingType<T>
    {
        T CreateFormattedValue(T originalValue, FieldToBeObfuscated field, Dictionary<string, object> metadataParameters);
    }
}
