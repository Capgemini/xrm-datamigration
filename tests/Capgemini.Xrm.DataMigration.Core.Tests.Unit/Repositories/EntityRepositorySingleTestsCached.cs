using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.Repositories.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class EntityRepositorySingleTestsCached : EntityRepositorySingleTests
    {
        public EntityRepositorySingleTestsCached()
            : base(RepositoryCachingMode.Lookup)
        {
        }
    }
}