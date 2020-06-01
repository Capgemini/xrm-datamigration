namespace Capgemini.Xrm.DataMigration.Config
{
    public interface IFileStoreReaderConfig
    {
        /// <summary>
        /// Gets or sets preFix for JSON files.
        /// </summary>
        string FilePrefix { get; set; }

        /// <summary>
        /// Gets or sets json folder with exported files.
        /// </summary>
        string JsonFolderPath { get; set; }
    }
}