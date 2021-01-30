using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace Capgemini.Xrm.DataMigration.Core.IntegrationTests
{
    [ExcludeFromCodeCoverage]
    public static class ConnectionHelper
    {
        public static IOrganizationService GetOrganizationalServiceSource()
        {
            return GetOrganizationalService(ConfigurationManager.ConnectionStrings["CrmSource"].ConnectionString);
        }

        public static IOrganizationService GetOrganizationalServiceTarget()
        {
            return GetOrganizationalService(ConfigurationManager.ConnectionStrings["CrmTarget"].ConnectionString);
        }

        private static IOrganizationService GetOrganizationalService(string connectionString)
        {
            if (!connectionString.ToUpper(CultureInfo.InvariantCulture).Contains("REQUIRENEWINSTANCE=TRUE"))
            {
                connectionString = $"RequireNewInstance=True; {connectionString}";
            }

            CrmServiceClient.MaxConnectionTimeout = new System.TimeSpan(1, 0, 0);
            return new CrmServiceClient(connectionString);
        }
    }
}