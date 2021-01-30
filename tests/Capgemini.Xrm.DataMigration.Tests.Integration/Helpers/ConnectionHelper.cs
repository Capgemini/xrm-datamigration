using System.Configuration;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace Capgemini.Xrm.DataMigration.IntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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

        public static IOrganizationService GetOrganizationalServicePortalSource()
        {
            return GetOrganizationalService(ConfigurationManager.ConnectionStrings["CrmPortalSource"].ConnectionString);
        }

        public static IOrganizationService GetOrganizationalServicePortalTarget()
        {
            return GetOrganizationalService(ConfigurationManager.ConnectionStrings["CrmPortalTarget"].ConnectionString);
        }

        public static IOrganizationService GetOrganizationSprintAutoTest()
        {
            return GetOrganizationalService(ConfigurationManager.ConnectionStrings["CrmSprintAuto"].ConnectionString);
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