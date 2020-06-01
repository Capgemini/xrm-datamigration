using System.Collections.Generic;

namespace Capgemini.Xrm.DataMigration.Config
{
    public interface IFileStoreWriterConfig : IFileStoreReaderConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether creates a seperate file per entity.
        /// </summary>
        bool SeperateFilesPerEntity { get; set; }

        /// <summary>
        /// Gets excluded fields from the output file.
        /// </summary>
        List<string> ExcludedFields { get; }
    }
}