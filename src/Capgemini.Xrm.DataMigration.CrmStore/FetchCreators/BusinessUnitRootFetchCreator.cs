using System.Text;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.Model;

namespace Capgemini.Xrm.DataMigration.CrmStore.FetchCreators
{
    /// <summary>
    /// this rules allowas mapping to rot BU. return aliased value in fomrat map.fieldname.isRootBu when lookup points to root BU. Whn it;s regular chuld BU, such enatry wil not be created.
    /// During import, if such value is encountred, lookup will be transalted to root BU.
    /// </summary>
    public class BusinessUnitRootFetchCreator : IMappingFetchCreator
    {
        public string GetExportFetchXML(string entityName, CrmField field)
        {
            string retValue = string.Empty;
            if (field?.LookupType == "businessunit")
            {
                StringBuilder fetch = new StringBuilder();
                fetch.AppendLine($@"<link-entity name='businessunit'  from='businessunitid' to='{field.FieldName}' link-type='outer'>");
                fetch.AppendLine($"  <attribute name='name' alias='map.{field.FieldName}.isRootBU.name' />");
                fetch.AppendLine("<filter>");
                fetch.AppendLine("  <condition attribute='parentbusinessunitid' operator='null' />");
                fetch.AppendLine("</filter>");
                fetch.Append("</link-entity>");

                retValue = fetch.ToString();
            }

            return retValue;
        }

        public bool UseForEntity(string entityName)
        {
            return true; // always map root BU!
        }
    }
}