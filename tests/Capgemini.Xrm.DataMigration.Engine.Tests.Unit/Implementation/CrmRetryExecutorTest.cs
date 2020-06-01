using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Resiliency;
using Capgemini.DataMigration.Resiliency.Polly;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.Implementation
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmRetryExecutorTest
    {
        private ServiceRetryExecutor executor;

        [TestMethod]
        public void CanExecuteFirstAttempt()
        {
            var p = new PollyFluentPolicy();

            executor = new ServiceRetryExecutor(p);
            var res = executor.Execute(() => { return 1 + 1; });

            Assert.AreEqual(2, res);
        }

        [TestMethod]
        public void CrmRetryExecutorImplementsIPolicyBuilderInterface()
        {
            var p = new PollyFluentPolicy();
            Assert.IsInstanceOfType(p, typeof(IPolicyBuilder<PollyFluentPolicy>));
        }
    }
}