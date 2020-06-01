using System.Xml.Serialization;

namespace Capgemini.Xrm.DataMigration.Model
{
    /// <summary>
    /// CRM SDK Configuration Migration Tool schema.
    /// </summary>
    [XmlRoot("relationship")]
    public class CrmRelationship
    {
        [XmlAttribute("name")]
        public string RelationshipName { get; set; }

        [XmlAttribute("manyToMany")]
        public bool ManyToMany { get; set; }

        [XmlAttribute("isreflexive")]
        public bool IsReflexive { get; set; }

        [XmlAttribute("relatedEntityName")]
        public string RelatedEntityName { get; set; }

        [XmlAttribute("m2mTargetEntity")]
        public string TargetEntityName { get; set; }

        [XmlAttribute("m2mTargetEntityPrimaryKey")]
        public string TargetEntityPrimaryKey { get; set; }
    }
}