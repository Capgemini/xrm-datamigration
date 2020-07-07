using System.Collections.Generic;
using Capgemini.Xrm.DataMigration.Config;

namespace Capgemini.Xrm.DataMigration.CrmStore.Config
{
    public class CrmStoreReaderConfig : ICrmStoreReaderConfig
    {
        private readonly List<string> fetchXMlQueries;

        public CrmStoreReaderConfig(List<string> fetchXMlQueries)
        {
            this.fetchXMlQueries = fetchXMlQueries;
        }

        public int BatchSize { get; set; } = 500;

        public bool OneEntityPerBatch { get; set; } = true;

        public int PageSize { get; set; } = 500;

        public int TopCount { get; set; } = 500;

        /// <summary>
        /// Gets or Sets the fields to Obfuscate.
        /// </summary>
        public Dictionary<string, List<string>> FieldsToObfuscate { get; set; } = null;

        public List<string> GetFetchXMLQueries()
        {
            return fetchXMlQueries;
        }
    }
}