using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capgemini.DataScrambler;
using Capgemini.DataScrambler.Scramblers;
using Capgemini.Xrm.DataMigration.Core;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace Capgemini.Xrm.DataMigration.Engine.Obfuscate
{
    public class CrmObfuscateDecimalHandler : ICrmObfuscateHandler
    {
        private ScramblerClient<decimal> decimalScramblerClient;

        public CrmObfuscateDecimalHandler()
        {
            this.decimalScramblerClient = new ScramblerClient<decimal>(new DecimalScrambler());
        }

        public bool CanHandle(Type type)
        {
            return type.Equals(typeof(decimal));
        }

        public void HandleObfuscation(Entity entity, string fieldName, IEntityMetadataCache metaData)
        {
            DecimalAttributeMetadata decimalMetaData = (DecimalAttributeMetadata) metaData.GetAttribute(entity.LogicalName, fieldName);
            int min = (int) decimalMetaData.MinValue.GetValueOrDefault(0);
            int max = (int) decimalMetaData.MaxValue.GetValueOrDefault(10);
            entity[fieldName] = decimalScramblerClient.ExecuteScramble((decimal)entity[fieldName], min, max);
        }
    }
}
