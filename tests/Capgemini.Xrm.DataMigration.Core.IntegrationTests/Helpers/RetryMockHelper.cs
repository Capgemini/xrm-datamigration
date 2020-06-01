using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Resiliency;
using Moq;

namespace Capgemini.Xrm.DataMigration.Core.IntegrationTests
{
    [ExcludeFromCodeCoverage]
    public static class RetryMockHelper
    {
        public static IRetryExecutor GetMockExecutor()
        {
            var mock = new Mock<IRetryExecutor>();
            return mock.Object;
        }
    }
}