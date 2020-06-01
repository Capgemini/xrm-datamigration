using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Capgemini.DataMigration.Core.Tests.Base
{
    [ExcludeFromCodeCoverage]
    public class TestOrganizationalService : IOrganizationService
    {
        public TestOrganizationalService()
        {
            EntityCollection = new EntityCollection();
        }

        public EntityCollection EntityCollection { get; set; }

        public OrganizationResponse ExecutionResponse { get; set; }

        public QueryExpression QueryExpression { get; set; }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            throw new NotImplementedException();
        }

        public Guid Create(Entity entity)
        {
            Console.WriteLine("Create invoked!");
            return entity?.Id ?? Guid.Empty;
        }

        public void Delete(string entityName, Guid id)
        {
            Console.WriteLine("Delete invoked!");
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            throw new NotImplementedException();
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            return ExecutionResponse;
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            return new Entity(entityName, id);
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            QueryExpression = (QueryExpression)query;
            return EntityCollection;
        }

        public void Update(Entity entity)
        {
            Console.WriteLine("Update invoked!");
        }
    }
}