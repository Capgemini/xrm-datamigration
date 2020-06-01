using System.Collections.Generic;
using Capgemini.Xrm.DataMigration.Config;

namespace Capgemini.Xrm.DataMigration.CrmStore.Config
{
    public class CrmStoreWriterConfig : ICrmStoreWriterConfig
    {
        public List<string> NoUpsertEntities { get; private set; } = new List<string>();

        public int SaveBatchSize { get; set; } = 500;

        public List<string> NoUpdateEntities { get; private set; } = new List<string>();
    }
}