using System;
using System.Collections.Generic;

namespace Capgemini.Xrm.DataMigration.Config
{
    [Serializable]
    public class ObjectTypeCodeMappingConfiguration
    {
        public ObjectTypeCodeMappingConfiguration()
        {
            EntityToTypeCodeMapping = new Dictionary<string, int>();
            FieldsToSearchForMapping = new List<string>();
        }

        /// <summary>
        /// Gets mapping of logical names to entity type codes in the source organisation.
        /// </summary>
        public Dictionary<string, int> EntityToTypeCodeMapping { get; }

        /// <summary>
        /// Gets a list of field logical names to map entity type codes for.
        /// </summary>
        public List<string> FieldsToSearchForMapping { get; }
    }
}