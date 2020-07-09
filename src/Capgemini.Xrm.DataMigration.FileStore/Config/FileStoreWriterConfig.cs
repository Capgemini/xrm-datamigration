using System.Collections.Generic;
using Capgemini.DataMigration.Core.Model;
using Capgemini.Xrm.DataMigration.Config;

namespace Capgemini.Xrm.DataMigration.FileStore.Config
{
    public class FileStoreWriterConfig : FileStoreReaderConfig, IFileStoreWriterConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether creates a seperate file per entity.
        /// </summary>
        public bool SeperateFilesPerEntity { get; set; } = true;

        /// <summary>
        /// Gets excluded Fields from the output file.
        /// </summary>
        public List<string> ExcludedFields { get; } = new List<string>();

        /// <summary>
        /// Gets the fields that must be obfuscated.
        /// </summary>
        public List<EntityToBeObfuscated> FieldsToObfuscate { get; } = new List<EntityToBeObfuscated>();
    }
}