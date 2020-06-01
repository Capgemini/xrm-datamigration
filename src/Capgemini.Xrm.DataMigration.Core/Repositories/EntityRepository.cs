using System;
using System.Collections.Generic;
using System.Linq;
using Capgemini.DataMigration.Exceptions;
using Capgemini.DataMigration.Resiliency;
using Capgemini.Xrm.DataMigration.Cache;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Extensions;
using Capgemini.Xrm.DataMigration.Model;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Capgemini.Xrm.DataMigration.Repositories
{
    public class EntityRepository : IEntityRepository
    {
        private readonly IEntityMetadataCache metadataCache;
        private readonly IOrganizationService orgService;
        private readonly IRetryExecutor retryExecutor;

        public EntityRepository(IOrganizationService orgService, IRetryExecutor retryExecutor)
            : this(orgService, retryExecutor, new EntityMetadataCache(orgService))
        {
        }

        public EntityRepository(IOrganizationService orgService, IRetryExecutor retryExecutor, IEntityMetadataCache entMetadataCache)
        {
            this.orgService = orgService;
            this.retryExecutor = retryExecutor;
            metadataCache = entMetadataCache;
        }

        /// <summary>
        /// Gets return current Organizational service.
        /// </summary>
        public IOrganizationService GetCurrentOrgService
        {
            get
            {
                return this.orgService;
            }
        }

        /// <summary>
        /// Gets the Entity Metadata Cache.
        /// </summary>
        public IEntityMetadataCache GetEntityMetadataCache
        {
            get
            {
                return metadataCache;
            }
        }

        /// <summary>
        /// Create or Update entities using UpsertRequest and ExecuteMultipleRequest.
        /// </summary>
        /// <param name="entities">entities.</param>
        public void CreateUpdateEntities(List<EntityWrapper> entities)
        {
            ExecuteMultipleWithRetry(entities, wrapper => new UpsertRequest
            {
                Target = wrapper.OriginalEntity
            });
        }

        /// <summary>
        /// Create Entities using CreateRequest and ExecuteMultiple.
        /// </summary>
        /// <param name="entities">entities.</param>
        public void CreateEntities(List<EntityWrapper> entities)
        {
            ExecuteMultipleWithRetry(entities, wrapper => new CreateRequest
            {
                Target = wrapper.OriginalEntity
            });
        }

        /// <summary>
        /// Update Entities using UpdateRequest and ExecuteMultiple.
        /// </summary>
        /// <param name="entities">entities.</param>
        public void UpdateEntities(List<EntityWrapper> entities)
        {
            ExecuteMultipleWithRetry(entities, wrapper => new UpdateRequest
            {
                Target = wrapper.OriginalEntity
            });
        }

        /// <summary>
        /// Assign Entities using AssignRequest and ExecuteMultiple.
        /// </summary>
        /// <param name="entities">entities.</param>
        public void AssignEntities(List<EntityWrapper> entities)
        {
            ExecuteMultipleWithRetry(entities, wrapper => new AssignRequest
            {
                Target = wrapper.OriginalEntity.ToEntityReference(),
                Assignee = wrapper.OriginalEntity.GetAttributeValue<EntityReference>(EntityFields.OwnerId)
            });
        }

        /// <summary>
        /// Associates Entities using AssociateRequest and ExecuteMultiple.
        /// </summary>
        /// <param name="entities">entities.</param>
        public void AssociateManyToManyEntity(List<EntityWrapper> entities)
        {
            ExecuteMultipleWithRetry(entities, wrapper =>
            {
                ManyToManyDetails details = metadataCache.GetManyToManyEntityDetails(wrapper.LogicalName);
                return new AssociateRequest
                {
                    Target = new EntityReference(details.Entity1LogicalName, wrapper.OriginalEntity.GetAttributeValue<Guid>(details.Entity1IntersectAttribute)),
                    Relationship = new Relationship(details.SchemaName),
                    RelatedEntities = new EntityReferenceCollection()
                    {
                        new EntityReference(
                            details.Entity2LogicalName,
                            wrapper.OriginalEntity.GetAttributeValue<Guid>(details.Entity2IntersectAttribute))
                    }
                };
            });
        }

        /// <summary>
        /// Executes fetchXMLQuery, return one page of data.
        /// </summary>
        /// <param name="fetchXMLQuery">A Query to execute.</param>
        /// <param name="pageNumber">Page Number, starts from 1.</param>
        /// <param name="pageSize">Page Size, max 5000.</param>
        /// <param name="pagingCookie">Paging cookie.</param>
        /// <returns>List of EntityWrapper.</returns>
        public List<EntityWrapper> GetEntitesByFetchXML(string fetchXMLQuery, int pageNumber, int pageSize, ref string pagingCookie)
        {
            var ec = new List<EntityWrapper>();

            var fetchXml = new FetchExpression(fetchXMLQuery)
                .ApplyPagingDetails(pagingCookie, pageNumber, pageSize);

            var retrieved = orgService.RetrieveMultiple(fetchXml);
            ec.AddRange(retrieved.Entities.Select(p => new EntityWrapper(p, metadataCache.GetManyToManyEntityDetails(p.LogicalName).IsManyToMany)).ToList());
            pagingCookie = retrieved.PagingCookie;

            // Remove Primary Attribute - not needed
            foreach (var item in ec)
            {
                EntityMetadata entMet = metadataCache.GetEntityMetadata(item.LogicalName);
                if (item.OriginalEntity.Attributes.ContainsKey(entMet.PrimaryIdAttribute))
                {
                    item.OriginalEntity.Attributes.Remove(entMet.PrimaryIdAttribute);
                }
            }

            return ec;
        }

        /// <summary>
        /// Returns Parent Busines Unit Id.
        /// </summary>
        /// <returns>A Guid for business unit.</returns>
        public Guid GetParentBuId()
        {
            var bu = orgService.GetEntitiesByColumn("businessunit", "parentbusinessunitid", null, new string[] { "businessunitid" }).Entities.FirstOrDefault();
            return bu.Id;
        }

        public List<EntityMetadata> GetAllEntitesMetadata()
        {
            RetrieveAllEntitiesRequest request = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.All
            };

            RetrieveAllEntitiesResponse response = (RetrieveAllEntitiesResponse)orgService.Execute(request);

            return response.EntityMetadata?.ToList();
        }

        /// <summary>
        /// Get entities by name.
        /// </summary>
        /// <param name="entityName">Entity Name.</param>
        /// <param name="collumns">Collumns to Get.</param>
        /// <param name="pageSize">Page Size.</param>
        /// <returns>value.</returns>
        public List<Entity> GetEntitiesByName(string entityName, string[] collumns, int pageSize)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = entityName,
                ColumnSet = collumns != null ? new ColumnSet(collumns) : new ColumnSet(true)
            };

            List<Entity> entities = orgService.GetDataByQuery(query, pageSize).Entities.ToList();

            return entities;
        }

        /// <summary>
        /// Deletes entity.
        /// </summary>
        /// <param name="entityName">Entity Name.</param>
        /// <param name="entityId">Entity Id.</param>
        public void DeleteEntity(string entityName, Guid entityId)
        {
            orgService.Delete(entityName, entityId);
        }

        public Guid GetOrganizationId()
        {
            WhoAmIRequest request = new WhoAmIRequest();
            WhoAmIResponse response = (WhoAmIResponse)orgService.Execute(request);
            return response.OrganizationId;
        }

        public List<Entity> FindEntitiesByName(string entityName, string nameValue)
        {
            var entMet = GetEntityMetadataCache.GetEntityMetadata(entityName);
            var rec = GetCurrentOrgService.GetEntitiesByColumn(entityName, entMet.PrimaryNameAttribute, nameValue, null, 10);
            return rec.Entities.ToList();
        }

        public Guid GetGuidForMapping(string entityName, string[] filterFields, object[] filterValues)
        {
            if (filterFields == null || filterValues == null || filterFields.Length != filterValues.Length)
            {
                throw new ArgumentException("filter fields must have same length as filter values!");
            }

            QueryExpression query = new QueryExpression
            {
                EntityName = entityName,
                ColumnSet = new ColumnSet(false)
            };

            for (int i = 0; i < filterFields.Length; i++)
            {
                if (!(filterValues[i] is EntityReference entRefValue))
                {
                    query.Criteria.AddCondition(filterFields[i], ConditionOperator.Equal, filterValues[i]);
                }
                else
                {
                    Guid refId = entRefValue.Id;
                    query.Criteria.AddCondition(filterFields[i], ConditionOperator.Equal, refId);
                }
            }

            List<Entity> entities = orgService.GetDataByQuery(query, 2).Entities.ToList();

            if (entities.Count > 1)
            {
                throw new ConfigurationException($"incorrect mapping value - cannot find unique record, Found {entities.Count} maching criteria {entityName}:{string.Join(",", filterFields)}={string.Join(", ", filterValues)}");
            }

            if (entities.Count == 0)
            {
                return Guid.Empty;
            }

            return entities[0].Id;
        }

        /// <summary>
        /// Returns the total number of records count.
        /// </summary>
        /// <param name="entityName">The entity name to retrieve.</param>
        /// <param name="filterFields">String array for the filter attributes.</param>
        /// <param name="filterValues">Object array for the filter values.</param>
        /// <returns>Returns the total number of records.</returns>
        public int GetTotalRecordCount(string entityName, string[] filterFields, object[] filterValues)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = entityName,
                ColumnSet = new ColumnSet(false)
            };

            if (filterFields != null && filterValues != null && filterFields.Length > 0 && filterValues.Length > 0)
            {
                if (filterFields.Length != filterValues.Length)
                {
                    throw new ArgumentException("filter fields must have same length as filter values!");
                }

                for (int i = 0; i < filterFields.Length; i++)
                {
                    query.Criteria.AddCondition(filterFields[i], ConditionOperator.Equal, filterValues[i]);
                }
            }

            return orgService.GetDataByQuery(query, 5000, false).TotalRecordCount;
        }

        protected static void PopulateExecutionResults(List<EntityWrapper> entities, ExecuteMultipleResponse responseWithResults)
        {
            if (responseWithResults == null)
            {
                throw new ArgumentNullException(nameof(responseWithResults));
            }

            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            if (responseWithResults.Responses != null)
            {
                foreach (var responseItem in responseWithResults.Responses)
                {
                    if (entities.Count > responseItem.RequestIndex)
                    {
                        var ent = entities[responseItem.RequestIndex];
                        ent.OperationType = responseItem.GetOperationType();
                        ent.OperationResult = responseItem.GetOperationMessage(ent.OriginalEntity);
                        ent.OperationErrorCode = responseItem.Fault?.ErrorCode;
                    }
                }
            }
        }

        protected virtual void ExecuteMultipleWithRetry(List<EntityWrapper> entities, Func<EntityWrapper, OrganizationRequest> orgRequest)
        {
            var requests = new OrganizationRequestCollection();

            requests.AddRange(entities.Select(wrapper => orgRequest(wrapper)).ToArray());

            var responseWithResults = retryExecutor.Execute(() => orgService.ExecuteMultiple(requests));

            PopulateExecutionResults(entities, responseWithResults);
        }
    }
}