using System;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using Capgemini.DataMigration.Resiliency.Polly;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.Implementation
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class PollyRetryBuilderTests
    {
        private int total;

        [TestInitialize]
        public void Setup()
        {
            total = 0;
        }

        [TestMethod]
        public void PollyFluentPolicyCanExecute()
        {
            var p = new PollyFluentPolicy();
            p.AddType<TimeoutException>().AddOrType<FaultException>();

            var res = p.Execute(() => { return 1 + 1; }, 5);

            Assert.AreEqual(2, res);
        }

        [TestMethod]
        public void PollyFluentPolicyCanReturnDefaultAfter5Failures()
        {
            var p = new PollyFluentPolicy();
            p.AddType<TimeoutException>().AddOrType<FaultException>();

            var res = p.Execute(FakeMethodThatFails, 5);

            Assert.AreEqual(0, res);  // As its int after 5 failures return default which is 0
            Assert.AreEqual(6, total);  // 5 retries plus the original attempt
        }

        public int FakeMethodThatFails()
        {
            total++;
            throw new TimeoutException("Took too long");
        }
    }
}