using System;
using System.Collections.Generic;
using System.Linq;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.Xrm.DataMigration.Core;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.MappingRules
{
    /// <summary>
    /// this rules allowas mapping to rot BU. return aliased value in fomrat map.fieldname.isRootBu when lookup points to root BU. Whn it;s regular chuld BU, such enatry wil not be created.
    /// During import, if such value is encountred, lookup will be transalted to root BU.
    /// </summary>
    public class BusinessUnitRootRule : IMappingRule
    {
        private readonly Guid parentBuId;

        public BusinessUnitRootRule(Guid parentBuId)
        {
            this.parentBuId = parentBuId;
        }

        public bool ProcessImport(string aliasedAttributeName, List<AliasedValue> values, out object replacementValue)
        {
            aliasedAttributeName.ThrowArgumentNullExceptionIfNull(nameof(aliasedAttributeName));

            string entName = values.Select(p => p.EntityLogicalName).Distinct().Single();

            bool processedValued = false;
            replacementValue = null;
            if (entName == "businessunit" && aliasedAttributeName.Split(new char[] { '.' })[2] == "isRootBU")
            {
                replacementValue = parentBuId;
                processedValued = true;
            }

            return processedValued;
        }
    }
}