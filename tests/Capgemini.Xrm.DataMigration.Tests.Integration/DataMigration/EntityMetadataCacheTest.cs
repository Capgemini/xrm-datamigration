using Capgemini.Xrm.DataMigration.Cache;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.IntegrationTests.DataMigration
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class EntityMetadataCacheTest
    {
        [TestMethod]
        public void GetEntityMetadata()
        {
            var orgService = ConnectionHelper.GetOrganizationalServiceTarget();
            var cache = new EntityMetadataCache(orgService);
            var contactCache = cache.GetEntityMetadata("contact");

            // this time it shoudl get item from cache
            var cache2 = new EntityMetadataCache(orgService);
            var contactCache2 = cache2.GetEntityMetadata("contact");

            Assert.AreSame(contactCache, contactCache2);
            Assert.AreEqual(contactCache.Keys.Length, contactCache2.Keys.Length);
        }
    }
}