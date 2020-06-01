using System;
using System.Collections.Generic;

namespace Capgemini.Xrm.DataMigration.Config
{
    [Serializable]
    public class MappingConfiguration
    {
        public bool ApplyAliasMapping { get; set; } = true;

        public Dictionary<string, Dictionary<Guid, Guid>> Mappings { get; } = new Dictionary<string, Dictionary<Guid, Guid>>();

        public string SourceRootBUName { get; set; }
    }
}