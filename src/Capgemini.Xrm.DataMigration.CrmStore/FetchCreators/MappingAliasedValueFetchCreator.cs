using System.Collections.Generic;
using System.Text;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.Model;

namespace Capgemini.Xrm.DataMigration.CrmStore.FetchCreators
{
    /// <summary>
    /// general mapping rules.
    /// </summary>
    public class MappingAliasedValueFetchCreator : IMappingFetchCreator
    {
        private readonly Dictionary<string, Dictionary<string, List<string>>> allLookupMappings;

        public MappingAliasedValueFetchCreator(Dictionary<string, Dictionary<string, List<string>>> lookupMappings)
        {
            allLookupMappings = lookupMappings;
        }

        public string GetExportFetchXML(string entityName, CrmField field)
        {
            field.ThrowArgumentNullExceptionIfNull(nameof(field));

            var lookupMapping = allLookupMappings.ContainsKey(entityName) ? allLookupMappings[entityName] : null;

            string retValue = string.Empty;

            if (lookupMapping != null && lookupMapping.ContainsKey(field.FieldName))
            {
                var rootBUfilter = string.Empty;
                if (field?.LookupType == "businessunit")
                {
                    StringBuilder fetch = new StringBuilder();
                    fetch.AppendLine("<filter>");
                    fetch.AppendLine("   <condition attribute='parentbusinessunitid' operator='not-null' />");
                    fetch.Append("</filter>");

                    rootBUfilter = fetch.ToString();
                }

                if (field.PrimaryKey)
                {
                    if (field.FieldName == "systemuserid")
                    {
                        entityName = "systemuser";
                    }

                    StringBuilder pkeyFetch = new StringBuilder();
                    pkeyFetch.AppendLine($@"<link-entity name='{entityName}' alias='map.{field.FieldName}.{entityName}' from='{field.FieldName}' to='{field.FieldName}' link-type='outer' >");

                    foreach (string atrFieldName in lookupMapping[field.FieldName])
                    {
                        pkeyFetch.AppendLine($"  <attribute name='{atrFieldName}' alias='map.{field.FieldName}.{entityName}.{atrFieldName}' />");
                    }

                    pkeyFetch.Append("</link-entity>");
                    retValue = pkeyFetch.ToString();
                }
                else
                {
                    StringBuilder fetch = new StringBuilder();
                    GenerateLinkEntityFetchXml(field, lookupMapping, rootBUfilter, fetch);
                    retValue = fetch.ToString();
                }
            }

            return retValue;
        }

        public bool UseForEntity(string entityName)
        {
            if (allLookupMappings == null)
            {
                return false;
            }

            return allLookupMappings.ContainsKey(entityName);
        }

        private static void GenerateLinkEntityFetchXml(CrmField field, Dictionary<string, List<string>> lookupMapping, string rootBUfilter, StringBuilder fetch)
        {
            fetch.AppendLine($@"<link-entity name='{field.LookupType}' alias='map.{field.FieldName}.{field.LookupType}' from='{field.LookupType}id' to='{field.FieldName}' link-type='outer' >");

            foreach (string atrFieldName in lookupMapping[field.FieldName])
            {
                fetch.AppendLine($"  <attribute name='{atrFieldName}' alias='map.{field.FieldName}.{field.LookupType}.{atrFieldName}' />");
            }

            if (!string.IsNullOrWhiteSpace(rootBUfilter))
            {
                fetch.AppendLine(rootBUfilter);
            }

            fetch.Append("</link-entity>");
        }
    }
}