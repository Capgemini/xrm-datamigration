using System;

namespace Capgemini.DataMigration.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class ValidatedNotNullAttribute : Attribute
    {
    }
}