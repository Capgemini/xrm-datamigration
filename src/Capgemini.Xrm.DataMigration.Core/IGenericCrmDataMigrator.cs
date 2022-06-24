using System.Threading;
using Capgemini.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Core
{
    public interface IGenericCrmDataMigrator : IGenericDataMigrator<Entity, EntityWrapper>
    {
    }
}