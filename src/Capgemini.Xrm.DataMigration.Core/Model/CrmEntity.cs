using System.Collections.Generic;
using System.Xml.Serialization;

namespace Capgemini.Xrm.DataMigration.Model
{
    /// <summary>
    /// CRM SDK Configuration Migration Tool schema.
    /// </summary>
    [XmlRoot("entity")]
    public class CrmEntity
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("displayname")]
        public string DisplayName { get; set; }

        [XmlAttribute("etc")]
        public string EntityCode { get; set; }

        [XmlAttribute("primaryidfield")]
        public string PrimaryIdField { get; set; }

        [XmlAttribute("primarynamefield")]
        public string PrimaryNameField { get; set; }

        [XmlAttribute("disableplugins")]
        public bool DisablePlugins { get; set; }

        [XmlArray("fields")]
        [XmlArrayItem("field")]
        public List<CrmField> CrmFields { get; } = new List<CrmField>();

        [XmlArray("relationships")]
        [XmlArrayItem("relationship")]
        public List<CrmRelationship> CrmRelationships { get; } = new List<CrmRelationship>();
    }
}