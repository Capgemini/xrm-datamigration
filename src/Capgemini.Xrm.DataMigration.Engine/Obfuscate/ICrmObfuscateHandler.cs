using Capgemini.Xrm.DataMigration.Core;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capgemini.Xrm.DataMigration.Engine.Obfuscate
{
    public interface ICrmObfuscateHandler
    {
        bool CanHandle(Type type);
        void HandleObfuscation(Entity entity, string fieldName, IEntityMetadataCache metaData);
    }
}
