using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Engine.DataProcessors;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.DataProcessors
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class EntityStatusProcessorTests
    {
        private EntityStatusProcessor systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            systemUnderTest = new EntityStatusProcessor(false, new List<string>());
        }

        [TestMethod]
        public void EntityStatusProcessor()
        {
            FluentActions.Invoking(() => new EntityStatusProcessor(false, new List<string>()))
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
            FluentActions.Invoking(() => systemUnderTest.ImportStarted())
                         .Should()
                         .NotThrow();
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