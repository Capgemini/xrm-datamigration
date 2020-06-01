using System;
using System.Collections.Generic;

namespace Capgemini.Xrm.DataMigration.FileStore.Model
{
    /// <summary>
    /// A class used to store all required data - schema od the file with data.
    /// </summary>
    [Serializable]
    public class CrmExportedDataStore
    {
        public CrmExportedDataStore()
        {
            ExportedEntities = new List<CrmEntityStore>();
        }

        public int RecordsCount { get; set; }

        public List<CrmEntityStore> ExportedEntities { get; }
    }
}