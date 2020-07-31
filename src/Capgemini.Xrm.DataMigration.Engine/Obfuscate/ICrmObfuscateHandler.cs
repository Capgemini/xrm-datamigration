using System;
using Capgemini.DataMigration.Core.Model;
using Capgemini.Xrm.DataMigration.Core;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.Engine.Obfuscate
{
    public interface ICrmObfuscateHandler
    {
        bool CanHandle(Type type);

        void HandleObfuscation(Entity entity, FieldToBeObfuscated field, IEntityMetadataCache metaData);
    }
}
