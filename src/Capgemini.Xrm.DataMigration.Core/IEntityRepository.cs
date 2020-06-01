using System;
using System.Collections.Generic;
using Capgemini.Xrm.DataMigration.DataStore;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace Capgemini.Xrm.DataMigration.Core
{
    /// <summary>
    /// Entity repository.
    /// </summary>
    public interface IEntityRepository
    {
        IOrganizationService GetCurrentOrgService { get; }

        IEntityMetadataCache GetEntityMetadataCache { get; }

        void CreateUpdateEntities(List<EntityWrapper> entities);

        void CreateEntities(List<EntityWrapper> entities);

        void UpdateEntities(List<EntityWrapper> entities);

        void AssignEntities(List<EntityWrapper> entities);

        void AssociateManyToManyEntity(List<EntityWrapper> entities);

        List<EntityWrapper> GetEntitesByFetchXML(string fetchXMLQuery, int pageNumber, int pageSize, ref string pagingCookie);

        List<EntityMetadata> GetAllEntitesMetadata();

        List<Entity> GetEntitiesByName(string entityName, string[] collumns, int pageSize);

        Guid GetParentBuId();

        void DeleteEntity(string entityName, Guid entityId);

        Guid GetOrganizationId();

        Guid GetGuidForMapping(string entityName, string[] filterFields, object[] filterValues);

        List<Entity> FindEntitiesByName(string entityName, string nameValue);
    }
}