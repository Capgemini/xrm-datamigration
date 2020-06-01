using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Core
{
    public interface IMappingRule
    {
        /// <summary>
        /// finds entity guid bsed on mapping data.
        /// </summary>
        /// <param name="aliasedAttributeName">aliasedAttributeName.</param>
        /// <param name="values">values.</param>
        /// <param name="replacementValue">replacementValue.</param>
        /// <returns>true if match was found and value should be replaced, false otherwise.</returns>
        bool ProcessImport(string aliasedAttributeName, List<AliasedValue> values, out object replacementValue);
    }
}