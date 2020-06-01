using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Moq;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.DataProcessors
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class WorkflowsPluginsProcessorTests : UnitTestBase
    {
        private Mock<IProcessRepository> mockProcessRepository;

        private WorkflowsPluginsProcessor systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();

            List<Tuple<string, string>> sdkSteps = new List<Tuple<string, string>>();
            List<string> workflows = new List<string>();
            mockProcessRepository = new Mock<IProcessRepository>();

            systemUnderTest = new WorkflowsPluginsProcessor(mockProcessRepository.Object, MockLogger.Object, sdkSteps, workflows);
        }

        [TestMethod]
        public void WorkflowsPluginsProcessor()
        {
            List<Tuple<string, string>> sdkSteps = new List<Tuple<string, string>>();
            List<string> workflows = new List<string>();

            FluentActions.Invoking(() => new WorkflowsPluginsProcessor(mockProcessRepository.Object, MockLogger.Object, sdkSteps, workflows))
                .Should()
                .NotThrow();
        }

        [TestMethod]
        public void ImportCompleted()
        {
            FluentActions.Invoking(() => systemUnderTest.ImportCompleted())
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ImportStarted()
        {
            var workflows = new List<Model.WorkflowEntity>();
            mockProcessRepository.Setup(a => a.GetAllWorkflows()).Returns(workflows);

            FluentActions.Invoking(() => systemUnderTest.ImportStarted())
                         .Should()
                         .NotThrow();

            mockProcessRepository.Verify(a => a.GetAllWorkflows());
        }

        [TestMethod]
        public void ProcessEntity()
        {
            EntityWrapper entity = new EntityWrapper(new Entity());
            int passNumber = 1;
            int maxPassNumber = 3;

            FluentActions.Invoking(() => systemUnderTest.ProcessEntity(entity, passNumber, maxPassNumber))
                         .Should()
                         .NotThrow();
        }
    }
}