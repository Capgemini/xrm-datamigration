using Capgemini.DataMigration.Core.Helpers;
using Capgemini.DataMigration.Core.Model;

namespace Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting.FormattingOptions
{
    public class FormattingOptionLookup
    {
        public virtual ObfuscationLookup GetObfuscationLookup(string fileName)
        {
            return ObfuscationLookupHelper.ObfuscationLookups[fileName];
        }
    }
}