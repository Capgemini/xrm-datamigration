using System.Collections.Generic;
using Capgemini.DataMigration.Core.Model;

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

        /// <summary>
        /// Gets the fields that must be obfuscated.
        /// </summary>
        List<EntityToBeObfuscated> FieldsToObfuscate { get; }
    }
}