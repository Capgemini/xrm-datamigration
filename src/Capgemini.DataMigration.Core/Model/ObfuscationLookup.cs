using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Capgemini.DataMigration.Core.Model
{
    public class ObfuscationLookup
    {
        private List<dynamic> dataRows = new List<dynamic>();

        public ObfuscationLookup(string name, List<dynamic> dataRows)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"{nameof(name)} must not be null or empty.");
            }

            if (dataRows == null || dataRows.Count.Equals(0))
            {
                throw new ArgumentException($"{nameof(dataRows)} must not be null or empty.");
            }

            Name = name;
            this.dataRows = dataRows;
        }

        public int Count
        {
            get { return dataRows.Count; }
        }

        public string Name { get; }

        public object this[int index, string columnname]
        {
            get
            {
                ExpandoObject row = dataRows[index];
                var properties = (IDictionary<string, object>)row;

                if (properties.ContainsKey(columnname))
                {
                    return properties[columnname];
                }

                return null;
            }
        }
    }
}
