using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.Model;

namespace Capgemini.Xrm.DataMigration.Config
{
    [Serializable]
    [XmlRoot("entities")]
    public class CrmSchemaConfiguration
    {
        public CrmSchemaConfiguration()
        {
            Entities = new List<CrmEntity>();
        }

        [XmlElement("entity")]
        public List<CrmEntity> Entities { get; }

        public static CrmSchemaConfiguration ReadFromFile(string filePath)
        {
            CrmSchemaConfiguration config;

            using (StreamReader reader = new StreamReader(filePath))
            {
                config = ReadFromStream(reader);
                reader.Close();
            }

            return config;
        }

        public static CrmSchemaConfiguration ReadFromStream(StreamReader stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CrmSchemaConfiguration));

            CrmSchemaConfiguration config = null;

            using (XmlReader reader = XmlReader.Create(stream))
            {
                config = (CrmSchemaConfiguration)serializer.Deserialize(reader);
            }

            return config;
        }

        public void SaveToFile(string filePath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(CrmSchemaConfiguration));
            using (TextWriter writer = new StreamWriter(filePath))
            {
                ser.Serialize(writer, this);
                writer.Close();
            }
        }

        public List<string> PrepareFetchXMLFromSchema(bool onlyActiveRecords, Dictionary<string, string> fetchXmlFilters, List<IMappingFetchCreator> mappingFetchCreators)
        {
            if (mappingFetchCreators == null)
            {
                mappingFetchCreators = new List<IMappingFetchCreator>();
            }

            List<string> fetchXMls = new List<string>();

            foreach (var entity in Entities)
            {
                fetchXMls.Add(BuildFetchXml(entity, onlyActiveRecords, fetchXmlFilters, mappingFetchCreators));

                if (entity.CrmRelationships != null)
                {
                    foreach (var relationship in entity.CrmRelationships)
                    {
                        fetchXMls.Add(BuildFetchXml(relationship, entity, mappingFetchCreators));
                    }
                }
            }

            return fetchXMls;
        }

        private static string BuildFetchXml(CrmEntity entity, bool onlyActiveRecords, Dictionary<string, string> fetchXmlFilters, List<IMappingFetchCreator> mappingFetchCreators)
        {
            StringBuilder fetchXML = new StringBuilder();
            fetchXML.AppendLine("<fetch version=\"1.0\" output-format=\"xml - platform\" mapping=\"logical\" distinct=\"false\">");
            fetchXML.AppendLine("<entity name=\"" + entity.Name + "\">");

            var aplicableFetchCreators = mappingFetchCreators.Where(mr => mr.UseForEntity(entity.Name)).ToList();

            foreach (var field in entity.CrmFields)
            {
                fetchXML.AppendLine("<attribute name=\"" + field.FieldName + "\" />");

                if (field.FieldType.Equals("entityreference", StringComparison.InvariantCulture) || field.PrimaryKey)
                {
                    foreach (var rule in aplicableFetchCreators)
                    {
                        fetchXML.AppendLine(rule.GetExportFetchXML(entity.Name, field));
                    }
                }
            }

            if (onlyActiveRecords)
            {
                fetchXML.AppendLine("<filter type=\"and\" >");
                fetchXML.AppendLine($"      <condition attribute=\"{EntityFields.StateCode}\" operator=\"eq\" value=\"0\" />");
                fetchXML.AppendLine("</filter>");
            }
            else if (fetchXmlFilters != null && fetchXmlFilters.ContainsKey(entity.Name))
            {
                fetchXML.AppendLine(fetchXmlFilters[entity.Name]);
            }

            fetchXML.AppendLine("</entity>");
            fetchXML.AppendLine("</fetch>");

            return fetchXML.ToString();
        }

        private static string BuildFetchXml(CrmRelationship relationship, CrmEntity entity, List<IMappingFetchCreator> mappingFetchCreators)
        {
            StringBuilder fetchXML = new StringBuilder();
            fetchXML.AppendLine("<fetch version=\"1.0\" output-format=\"xml - platform\" mapping=\"logical\" distinct=\"false\">");
            fetchXML.AppendLine("<entity name=\"" + relationship.RelatedEntityName + "\">");

            fetchXML.AppendLine("<attribute name=\"" + entity.PrimaryIdField + "\" />");
            fetchXML.AppendLine("<attribute name=\"" + relationship.TargetEntityPrimaryKey + "\" />");

            var aplicableFetchCreators = mappingFetchCreators.Where(mr => mr.UseForEntity(relationship.RelatedEntityName)).ToList();

            foreach (var rule in aplicableFetchCreators)
            {
                fetchXML.Append(rule.GetExportFetchXML(relationship.RelatedEntityName, new CrmField() { LookupType = entity.Name, FieldName = entity.PrimaryIdField }));
            }

            foreach (var rule in aplicableFetchCreators)
            {
                fetchXML.Append(rule.GetExportFetchXML(relationship.RelatedEntityName, new CrmField() { LookupType = relationship.TargetEntityName, FieldName = relationship.TargetEntityPrimaryKey }));
            }

            fetchXML.AppendLine("</entity>");
            fetchXML.AppendLine("</fetch>");

            return fetchXML.ToString();
        }
    }
}