using System;
using System.Collections.Generic;
using System.Linq;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Exceptions;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Model;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.DataProcessors
{
    public class WorkflowsPluginsProcessor : IEntityProcessor<Entity, EntityWrapper>
    {
        private readonly ILogger logger;
        private readonly IProcessRepository processRepository;

        private readonly List<Guid> stoppedWorkflows;
        private readonly List<Guid> stoppedPlugins;

        private List<Tuple<string, string>> sdkSteps;
        private List<string> workflows;

        public WorkflowsPluginsProcessor(IProcessRepository processRepository, ILogger logger, List<Tuple<string, string>> sdkSteps, List<string> workflows)
        {
            this.processRepository = processRepository;
            this.logger = logger;
            this.sdkSteps = sdkSteps;
            this.workflows = workflows;

            stoppedWorkflows = new List<Guid>();
            stoppedPlugins = new List<Guid>();
        }

        public int MinRequiredPassNumber
        {
            get
            {
                return 1;
            }
        }

        public void ImportCompleted()
        {
            StartWorkflows();
            StartPlugins();
        }

        public void ImportStarted()
        {
            StopWorkflows();
            StopPlugins();
        }

        public void ProcessEntity(EntityWrapper entity, int passNumber, int maxPassNumber)
        {
            // Nothing to be done
        }

        private void StartPlugins()
        {
            logger.LogVerbose("Starting stopped plugins");

            foreach (var id in stoppedPlugins)
            {
                try
                {
                    processRepository.ActivateSDKStep(id);
                    logger.LogVerbose($"Plugin {id} has been started");
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Cannot start plugin {ex.Message}");
                }
            }

            stoppedPlugins.Clear();
        }

        private void StartWorkflows()
        {
            logger.LogVerbose("Starting stopped workflows");

            foreach (var id in stoppedWorkflows)
            {
                try
                {
                    processRepository.ActivateWorkflow(id);
                    logger.LogVerbose($"Workflows {id} has been started");
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Cannot start workflow {ex.Message}");
                }
            }

            stoppedWorkflows.Clear();
        }

        private void StopPlugins()
        {
            logger.LogVerbose("Stopping configured plugins");

            if (stoppedPlugins.Count > 0)
            {
                throw new ValidationException("There are alredy stopped plugins, activate them first!");
            }

            List<SdkStep> allPlugins = processRepository.GetAllCustomizableSDKSteps();
            if (sdkSteps == null)
            {
                sdkSteps = allPlugins.Select(p => new Tuple<string, string>(p.Name, p.Handler)).ToList();
            }

            if (sdkSteps != null && sdkSteps.Count > 0)
            {
                foreach (var item in sdkSteps)
                {
                    SdkStep plugin = allPlugins.FirstOrDefault(p => p.Name == item.Item1 && p.Handler == item.Item2);
                    if (plugin != null && plugin.SDKSepStatusCode == SdkSepStatusCode.Enabled)
                    {
                        DeactivateSdkStep(plugin);
                    }
                    else
                    {
                        logger.LogVerbose($"Plugin {item.Item1}:{item.Item2} does not exist or is already stopped");
                    }
                }
            }
        }

        private void DeactivateSdkStep(SdkStep plugin)
        {
            try
            {
                processRepository.DeActivateSDKStep(plugin.Id);
                stoppedPlugins.Add(plugin.Id);
                logger.LogVerbose($"Plugin {plugin.Name}:{plugin.Handler} has been stopped");
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Cannot stop plugin {ex.Message}:{plugin.Name} error: {plugin.Handler}");
            }
        }

        private void StopWorkflows()
        {
            logger.LogVerbose("Stopping configured workflows");

            if (stoppedWorkflows.Count > 0)
            {
                throw new ValidationException("There are alredy stopped workflows, activate them first!");
            }

            List<WorkflowEntity> allWorkflows = processRepository.GetAllWorkflows().Where(p => p.WfType == WorkflowType.Definition && (p.WfCategory == WorkflowCategory.Workflow || p.WfCategory == WorkflowCategory.BusinessRule)).ToList();
            if (workflows == null)
            {
                workflows = allWorkflows.Select(p => p.Name).ToList();
            }

            if (workflows != null && workflows.Count > 0)
            {
                foreach (var item in workflows)
                {
                    WorkflowEntity wkflow = allWorkflows.FirstOrDefault(p => p.Name == item);
                    if (wkflow != null && wkflow.WfState == WorkflowState.Activated)
                    {
                        DeactivateWorkflow(wkflow);
                    }
                    else
                    {
                        logger.LogVerbose($"Workflows {item} does not exist or is already stopped");
                    }
                }
            }
        }

        private void DeactivateWorkflow(WorkflowEntity wkflow)
        {
            try
            {
                processRepository.DeActivateWorkflow(wkflow.Id);
                stoppedWorkflows.Add(wkflow.Id);
                logger.LogVerbose($"Workflows {wkflow.Name} has been stopped");
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Cannot stop workflow {wkflow.Name} error {ex.Message} ");
            }
        }
    }
}