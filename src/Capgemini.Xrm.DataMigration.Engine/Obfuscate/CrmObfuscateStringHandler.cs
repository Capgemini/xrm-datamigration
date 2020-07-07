using Capgemini.DataScrambler;
using Capgemini.DataScrambler.Scramblers;
using Capgemini.Xrm.DataMigration.Core;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capgemini.Xrm.DataMigration.Engine.Obfuscate
{
    public class CrmObfuscateStringHandler : ICrmObfuscateHandler
    {
        private ScramblerClient<string> strScramblerClient;

        public CrmObfuscateStringHandler()
        {
            this.strScramblerClient = new ScramblerClient<string>(new StringScrambler());
        }

        public bool CanHandle(Type type)
        {
            return type.Equals(typeof(string));
        }

        public void HandleObfuscation(Entity entity, string fieldName, IEntityMetadataCache metaData)
        {
            StringAttributeMetadata stringMetaData = (StringAttributeMetadata)metaData.GetAttribute(entity.LogicalName, fieldName);
            entity[fieldName] = strScramblerClient.ExecuteScramble((string)entity[fieldName]);
        }
    }
}
