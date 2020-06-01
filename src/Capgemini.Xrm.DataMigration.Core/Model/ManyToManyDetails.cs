namespace Capgemini.Xrm.DataMigration.Model
{
    public class ManyToManyDetails
    {
        public bool IsManyToMany { get; set; }

        public string SchemaName { get; set; }

        public string Entity1LogicalName { get; set; }

        public string Entity2LogicalName { get; set; }

        public string Entity1IntersectAttribute { get; set; }

        public string Entity2IntersectAttribute { get; set; }
    }
}