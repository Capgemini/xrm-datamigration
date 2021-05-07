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
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Repositories
{
    public class EntityRepositorySingle : EntityRepository
    {
        public EntityRepositorySingle(IOrganizationService orgService, IRetryExecutor retryExecutor, RepositoryCachingMode cachingMode)
            : this(orgService, retryExecutor, new EntityMetadataCache(orgService), cachingMode)
        {
        }

        public EntityRepositorySingle(IOrganizationService orgService, IRetryExecutor retryExecutor, IEntityMetadataCache entMetadataCache, RepositoryCachingMode cachingMode)
            : base(orgService, retryExecutor, entMetadataCache, cachingMode)
        {
        }

        protected static void PopulateExecutionResults(List<EntityWrapper> entities, List<ExecuteMultipleResponseItem> responseWithResults)
        {
            responseWithResults.ThrowArgumentNullExceptionIfNull(nameof(responseWithResults));
            entities.ThrowArgumentNullExceptionIfNull(nameof(entities));

            foreach (ExecuteMultipleResponseItem responseItem in responseWithResults)
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

        protected override void ExecuteMultipleWithRetry(List<EntityWrapper> entities, Func<EntityWrapper, OrganizationRequest> orgRequest)
        {
            int cnt = 0;
            var requests = new OrganizationRequestCollection();
            var responseWithResults = new List<ExecuteMultipleResponseItem>();

            requests.AddRange(entities.Select(wrapper => orgRequest(wrapper)).ToArray());

            foreach (OrganizationRequest request in requests)
            {
                try
                {
                    OrganizationResponse response = this.GetCurrentOrgService.Execute(request);
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