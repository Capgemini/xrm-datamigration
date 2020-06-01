using Capgemini.Xrm.DataMigration.Config;

namespace Capgemini.Xrm.DataMigration.FileStore.Config
{
    public class FileStoreReaderConfig : IFileStoreReaderConfig
    {
        /// <summary>
        /// Gets or sets preFix for JSON files.
        /// </summary>
        public string FilePrefix { get; set; } = "ExportedData";

        /// <summary>
        /// Gets or sets json folder with exported files.
        /// </summary>
        public string JsonFolderPath { get; set; }
    }
}