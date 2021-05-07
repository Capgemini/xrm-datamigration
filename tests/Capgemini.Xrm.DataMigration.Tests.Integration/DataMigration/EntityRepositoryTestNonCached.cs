using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Capgemini.DataMigration.Resiliency;
using Capgemini.DataMigration.Resiliency.Polly;
using Capgemini.Xrm.DataMigration.Cache;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Capgemini.Xrm.DataMigration.IntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public abstract class EntityRepositoryTestNonCached : EntityRepositoryTest
    {
        public EntityRepositoryTestNonCached()
            : base(RepositoryCachingMode.None)
        {
        }
    }
}