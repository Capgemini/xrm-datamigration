using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.Repositories.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class EntityRepositoryTestsUncached : EntityRepositoryTests
    {
        public EntityRepositoryTestsUncached()
            : base(RepositoryCachingMode.None)
        {
        }
    }
}