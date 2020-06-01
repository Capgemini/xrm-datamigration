using System;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.DataStore;
using Microsoft.Xrm.Sdk;

namespace Capgemini.DataMigration.Core.Tests.Base
{
    [ExcludeFromCodeCoverage]
    public class TestMigrationEntityWrapper : MigrationEntityWrapper<Entity>
    {
        public TestMigrationEntityWrapper(Entity entity, bool isManyToMany = false)
            : base(entity, isManyToMany)
        {
        }

        public override Guid Id => Guid.NewGuid();

        public override string LogicalName => nameof(TestMigrationEntityWrapper);

        public override int AttributesCount => 1;
    }
}