using Capgemini.Xrm.DataMigration.Model;

namespace Capgemini.Xrm.DataMigration.Core
{
    public interface IMappingFetchCreator
    {
        /// <summary>
        /// produces bit of fetch xml which queries dayabase for mapping data.
        /// </summary>
        /// <param name="entityName">entityName.</param>
        /// <param name="field">field.</param>
        /// <returns>value.</returns>
        string GetExportFetchXML(string entityName, CrmField field);

        /// <summary>
        /// used to determine of processor neds to be executed for given enbtityt - help export performance.
        /// </summary>
        /// <param name="entityName">entityName.</param>
        /// <returns>value.</returns>
        bool UseForEntity(string entityName);
    }
}