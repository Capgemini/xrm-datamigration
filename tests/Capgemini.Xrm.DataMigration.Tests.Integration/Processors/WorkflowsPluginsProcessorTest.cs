using System;
using System.Collections.Generic;
using Capgemini.DataMigration.Core;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using Capgemini.Xrm.DataMigration.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.IntegrationTests.Processors
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class WorkflowsPluginsProcessorTest
    {
        [TestMethod]
        public void AllProcessingTest()
        {
            ConsoleLogger.LogLevel = 5;

            var target = ConnectionHelper.GetOrganizationalServiceSource();

            WorkflowsPluginsProcessor processor = new WorkflowsPluginsProcessor(new ProcessRepository(target), new ConsoleLogger(), null, null);

            processor.ImportStarted();

            processor.ImportCompleted();

            Assert.IsNotNull(processor);
        }

        [TestMethod]
        public void SomeProcessingTest()
        {
            ConsoleLogger.LogLevel = 5;

            var target = ConnectionHelper.GetOrganizationalServiceSource();

            List<Tuple<string, string>> plugins = new List<Tuple<string, string>>() { new Tuple<string, string>("Collection Plan De-Dupe : Create of nhs_supplyplan", "Nhsbt.Donor.Plugins.SupplyPlanDeDuplicate") };
            List<string> wflows = new List<string>() { "Appointment – Donor – Blood – Reminder Parent – 1 Day" };

            WorkflowsPluginsProcessor processor = new WorkflowsPluginsProcessor(new ProcessRepository(target), new ConsoleLogger(), plugins, wflows);

            processor.ImportStarted();

            processor.ImportCompleted();

            Assert.IsNotNull(processor);
        }
    }
}