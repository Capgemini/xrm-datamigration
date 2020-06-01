using System;
using System.Collections.Generic;
using System.Linq;
using Capgemini.Xrm.DataMigration.Core;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.MappingRules
{
    /// <summary>
    /// general mapping rules.
    /// </summary>
    public class MappingAliasedValueRule : IMappingRule
    {
        private readonly IEntityRepository entityRepository;

        public MappingAliasedValueRule(IEntityRepository entityRepository)
        {
            this.entityRepository = entityRepository;
        }

        public bool ProcessImport(string aliasedAttributeName, List<AliasedValue> values, out object replacementValue)
        {
            string entName = values.Select(p => p.EntityLogicalName).Distinct().Single();

            List<string> attrNames = values.Select(p => p.AttributeLogicalName).ToList();

            List<object> attrValues = values.Select(p => p.Value).ToList();

            replacementValue = entityRepository.GetGuidForMapping(entName, attrNames.ToArray(), attrValues.ToArray());

            return (Guid)replacementValue != Guid.Empty;
        }
    }
}