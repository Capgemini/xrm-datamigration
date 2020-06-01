using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Capgemini.Xrm.DataMigration.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Query;

namespace Capgemini.Xrm.DataMigration.Core.IntegrationTests.ExtensionTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class OrgServiceExtensionsTest
    {
        [TestMethod]
        public void GetDataByQueryNoEntitiesReturned()
        {
            var service = ConnectionHelper.GetOrganizationalServiceSource();

            var watch = Stopwatch.StartNew();

            QueryExpression exp = new QueryExpression("contact")
            {
                ColumnSet = new ColumnSet(false)
            };
            Microsoft.Xrm.Sdk.EntityCollection result = null;

            FluentActions.Invoking(() => result = service.GetDataByQuery(exp, 5000, false))
                            .Should()
                            .NotThrow();

            watch.Stop();
            Debug.WriteLine($"Count took {watch.Elapsed.Seconds} seconds, result {result.TotalRecordCount}");

            result.Should().NotBeNull();
        }
    }
}