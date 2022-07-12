using System;
using Capgemini.Xrm.DataMigration.Model;
using Microsoft.Xrm.Sdk.Metadata;

namespace Capgemini.Xrm.DataMigration.Core
{
    public interface IEntityMetadataCache
    {
        EntityMetadata GetEntityMetadata(string entityName);

        ManyToManyDetails GetManyToManyEntityDetails(string intersectEntityName);

        Type GetAttributeDotNetType(string entityName, string attributeName);

        AttributeMetadata GetAttribute(string entityName, string attributeName);

        bool ContainsAttribute(string entityName, string attributeName);

        string GetLookUpEntityName(string entityName, string attributeName);

        string GetIdAliasKey(string entName);
    }
}