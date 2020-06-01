using System;
using System.Collections.Generic;
using System.Linq;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.Extensions;
using Capgemini.Xrm.DataMigration.Model;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Capgemini.Xrm.DataMigration.Repositories
{
    public class ProcessRepository : IProcessRepository
    {
        private readonly IOrganizationService orgService;

        public ProcessRepository(IOrganizationService orgService)
        {
            this.orgService = orgService;
        }

        public void ActivateWorkflow(Guid id)
        {
            var activateRequest = new SetStateRequest
            {
                EntityMoniker = new EntityReference("workflow", id),
                State = new OptionSetValue((int)WorkflowState.Activated),
                Status = new OptionSetValue((int)WorkflowStatusCode.Activated)
            };

            this.orgService.Execute(activateRequest);
        }

        public void DeActivateWorkflow(Guid id)
        {
            var activateRequest = new SetStateRequest
            {
                EntityMoniker = new EntityReference("workflow", id),
                State = new OptionSetValue((int)WorkflowState.Draft),
                Status = new OptionSetValue((int)WorkflowStatusCode.Draft)
            };

            this.orgService.Execute(activateRequest);
        }

        public List<WorkflowEntity> GetAllWorkflows()
        {
            QueryExpression query = new QueryExpression { EntityName = "workflow", ColumnSet = new ColumnSet("name", EntityFields.StatusCode, EntityFields.StateCode, "type", "category", "rendererobjecttypecode") };

            EntityCollection results = orgService.GetDataByQuery(query, 5000);

            List<WorkflowEntity> processes = results.Entities.Select(p =>
           new WorkflowEntity(
               p.Id,
               p.GetAttributeValue<string>("name"),
               p.GetAttributeValue<string>("rendererobjecttypecode"),
               p.GetAttributeValue<OptionSetValue>("type"),
               p.GetAttributeValue<OptionSetValue>("category"),
               p.GetAttributeValue<OptionSetValue>(EntityFields.StatusCode),
               p.GetAttributeValue<OptionSetValue>(EntityFields.StateCode))).ToList();

            return processes;
        }

        public List<SdkStep> GetAllCustomizableSDKSteps()
        {
            QueryExpression query = new QueryExpression { EntityName = "sdkmessageprocessingstep", ColumnSet = new ColumnSet("name", "eventhandler", EntityFields.StatusCode, EntityFields.StateCode) };
            query.Criteria = new FilterExpression(LogicalOperator.And);
            query.Criteria.AddCondition(new ConditionExpression("ishidden", ConditionOperator.Equal, false));
            query.Criteria.AddCondition(new ConditionExpression("iscustomizable", ConditionOperator.Equal, true));

            EntityCollection results = orgService.GetDataByQuery(query, 5000);

            return results.Entities.Select(p => new SdkStep
            {
                Id = p.Id,
                Name = p.GetAttributeValue<string>("name"),
                Handler = p.GetAttributeValue<EntityReference>("eventhandler")?.Name,
                SDKSepStatusCode = (SdkSepStatusCode)p.GetAttributeValue<OptionSetValue>(EntityFields.StatusCode).Value,
                SDKStepState = (SdkStepState)p.GetAttributeValue<OptionSetValue>(EntityFields.StateCode).Value
            }).ToList();
        }

        public void ActivateSDKStep(Guid id)
        {
            var activateRequest = new SetStateRequest
            {
                EntityMoniker = new EntityReference("sdkmessageprocessingstep", id),
                State = new OptionSetValue((int)SdkStepState.Enabled),
                Status = new OptionSetValue((int)SdkSepStatusCode.Enabled)
            };

            this.orgService.Execute(activateRequest);
        }

        public void DeActivateSDKStep(Guid id)
        {
            var activateRequest = new SetStateRequest
            {
                EntityMoniker = new EntityReference("sdkmessageprocessingstep", id),
                State = new OptionSetValue((int)SdkStepState.Disabled),
                Status = new OptionSetValue((int)SdkSepStatusCode.Disabled)
            };

            this.orgService.Execute(activateRequest);
        }
    }
}