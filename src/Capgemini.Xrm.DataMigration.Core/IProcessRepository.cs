using System;
using System.Collections.Generic;
using Capgemini.Xrm.DataMigration.Model;

namespace Capgemini.Xrm.DataMigration.Core
{
    public interface IProcessRepository
    {
        void ActivateWorkflow(Guid id);

        void DeActivateWorkflow(Guid id);

        List<WorkflowEntity> GetAllWorkflows();

        List<SdkStep> GetAllCustomizableSDKSteps();

        void ActivateSDKStep(Guid id);

        void DeActivateSDKStep(Guid id);
    }
}