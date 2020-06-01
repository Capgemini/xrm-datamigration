using System.Collections.Generic;

namespace Capgemini.Xrm.DataMigration.Config
{
    public interface ICrmStoreWriterConfig
    {
        /// <summary>
        /// Gets don't use Upsert request, use Create and Update requests instead.
        /// </summary>
        List<string> NoUpsertEntities { get; }

        /// <summary>
        /// Gets don't use upsert or create requests - create only.
        /// </summary>
        List<string> NoUpdateEntities { get; }

        /// <summary>
        /// Gets or sets batch size used for executemultiple request.
        /// </summary>
        int SaveBatchSize { get; set; }
    }
}