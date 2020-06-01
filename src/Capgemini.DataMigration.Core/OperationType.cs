namespace Capgemini.DataMigration.Core
{
    /// <summary>
    /// Operation Types for migration engine.
    /// </summary>
    public enum OperationType
    {
        Create,
        Update,
        Associate,
        Assign,
        Failed,
        Ignore,
        New
    }
}