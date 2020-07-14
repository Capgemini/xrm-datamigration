using System;
using System.Collections.Generic;
using System.Globalization;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataScrambler;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting.FormattingOptions;

namespace Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting
{
    public class ObfuscationFormattingDouble : IObfuscationFormattingType<double>
    {
        public double CreateFormattedValue(double originalValue, FieldToBeObfuscated field)
        {
            throw new NotImplementedException();
        }
    }
}
