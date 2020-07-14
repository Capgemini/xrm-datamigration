using Capgemini.DataMigration.Core.Model;
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
    public class CrmObfuscateDoubleHandler : ICrmObfuscateHandler
    {
        private ScramblerClient<double> doubleScramblerClient;

        public CrmObfuscateDoubleHandler()
        {
            this.doubleScramblerClient = new ScramblerClient<double>(new DoubleScrambler());
        }

        public bool CanHandle(Type type)
        {
            return type.Equals(typeof(double));
        }

        public void HandleObfuscation(Entity entity, FieldToBeObfuscated field, IEntityMetadataCache metaData)
        {
            // Get the min and maximum values for the field using the meta data cache
            DoubleAttributeMetadata doubleMetaData = (DoubleAttributeMetadata)metaData.GetAttribute(entity.LogicalName, field.FieldName);
            int min = (int)doubleMetaData.MinValue.GetValueOrDefault(0);
            int max = (int)doubleMetaData.MaxValue.GetValueOrDefault(10);
            entity[field.FieldName] = doubleScramblerClient.ExecuteScramble((double)entity[field.FieldName], min, max);
        }
    }
}
