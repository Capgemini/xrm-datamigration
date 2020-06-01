using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.FileStore.Model
{
    /// <summary>
    /// CrmAttributeStore.
    /// </summary>
    [Serializable]
    public class CrmAttributeStore
    {
        public CrmAttributeStore()
        {
        }

        public CrmAttributeStore(KeyValuePair<string, object> attribute)
        {
            AttributeName = attribute.Key;
            AttributeType = attribute.Value.GetType().ToString();

            if (AttributeType == "Microsoft.Xrm.Sdk.EntityCollection")
            {
                var ec = (EntityCollection)attribute.Value;
                AttributeValue = ec.Entities.Select(e => new CrmEntityStore(e)).ToList();
            }
            else
            {
                AttributeValue = attribute.Value;
            }
        }

        public string AttributeName { get; set; }

        public object AttributeValue { get; set; }

        public string AttributeType { get; set; }
    }
}