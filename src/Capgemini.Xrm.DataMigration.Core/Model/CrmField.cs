using System.Xml.Serialization;

namespace Capgemini.Xrm.DataMigration.Model
{
    /// <summary>
    /// CRM SDK Configuration Migration Tool schema.
    /// </summary>
    [XmlRoot("field")]
    public class CrmField
    {
        [XmlAttribute("displayname")]
        public string DisplayName { get; set; }

        [XmlAttribute("name")]
        public string FieldName { get; set; }

        [XmlAttribute("type")]
        public string FieldType { get; set; }

        [XmlAttribute("primaryKey")]
        public bool PrimaryKey { get; set; }

        [XmlAttribute("lookupType")]
        public string LookupType { get; set; }

        [XmlAttribute("customfield")]
        public bool CustomField { get; set; }
    }
}