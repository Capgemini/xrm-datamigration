using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.DataMigration.Resiliency;
using Capgemini.Xrm.DataMigration.Cache;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Extensions;
using Capgemini.Xrm.DataMigration.Repositories;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Capgemini.Xrm.DataMigration.IntegrationTests
{
    /// <summary>
    /// No ExecuteMultiple used - fake.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class EntityRepositoryFake : EntityRepository
    {
        public EntityRepositoryFake(IOrganizationService orgService, IRetryExecutor retryExecutor, RepositoryCachingMode cachingMode)
            : this(orgService, retryExecutor, new EntityMetadataCache(orgService), cachingMode)
        {
        }

        public EntityRepositoryFake(IOrganizationService orgService, IRetryExecutor retryExecutor, IEntityMetadataCache entMetadataCache, RepositoryCachingMode cachingMode)
            : base(orgService, retryExecutor, entMetadataCache, cachingMode)
        {
        }

        protected static void PopulateExecutionResults(List<EntityWrapper> entities, List<ExecuteMultipleResponseItem> responseWithResults)
        {
            entities.ThrowArgumentNullExceptionIfNull(nameof(entities));
            responseWithResults.ThrowArgumentNullExceptionIfNull(nameof(responseWithResults));

            foreach (ExecuteMultipleResponseItem responseItem in responseWithResults)
            {
                if (entities.Count > responseItem.RequestIndex)
                {
                    var ent = entities[responseItem.RequestIndex];
                    ent.OperationType = responseItem.GetOperationType();
                    ent.OperationResult = "Fake";
                }
            }
        }

        protected override void ExecuteMultipleWithRetry(List<EntityWrapper> entities, Func<EntityWrapper, OrganizationRequest> orgRequest)
        {
            var requests = new OrganizationRequestCollection();

            requests.AddRange(entities.Select(wrapper => orgRequest(wrapper)).ToArray());

            int cnt = 0;

            List<ExecuteMultipleResponseItem> responseWithResults = new List<ExecuteMultipleResponseItem>();

            foreach (OrganizationRequest request in requests)
            {
                try
                {
                    OrganizationResponse response = new UpsertResponse() { };
                    responseWithResults.Add(new ExecuteMultipleResponseItem() { Response = response, RequestIndex = cnt });
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    responseWithResults.Add(new ExecuteMultipleResponseItem() { Fault = ex.Detail, RequestIndex = cnt });
                }

                cnt++;
            }

            PopulateExecutionResults(entities, responseWithResults);
        }
    }
}