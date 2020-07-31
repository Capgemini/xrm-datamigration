using Capgemini.DataMigration.Core.Helpers;
using Capgemini.DataMigration.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
