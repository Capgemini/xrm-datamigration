using System.Collections.Generic;
using Capgemini.DataMigration.Core.Model;
using Capgemini.Xrm.DataMigration.Core;

namespace Capgemini.Xrm.DataMigration.Config
{
    public interface ICrmStoreReaderConfig
    {
        /// <summary>
        /// Gets or sets batch Size - how many items stored in one json file.
        /// </summary>
        int BatchSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether if true, then one entity type is returned per batch
        /// If false, then multiple entity types are returned.
        /// </summary>
        bool OneEntityPerBatch { get; set; }

        /// <summary>
        /// Gets or sets read page size - max 5000.
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// Gets or sets max Limit of items per entity.
        /// </summary>
        int TopCount { get; set; }

        /// <summary>
        /// Gets the fields to Obfuscate.
        /// </summary>
        List<EntityToBeObfuscated> FieldsToObfuscate { get; }

        /// <summary>
        /// Generates FetchXMLQueries.
        /// </summary>
        /// <returns>Returns fetchxml list.</returns>
        List<string> GetFetchXMLQueries(IEntityMetadataCache entityMetadataCache);
    }
}