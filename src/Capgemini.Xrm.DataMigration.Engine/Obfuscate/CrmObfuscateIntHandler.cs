using System;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataScrambler;
using Capgemini.DataScrambler.Scramblers;
using Capgemini.Xrm.DataMigration.Core;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace Capgemini.Xrm.DataMigration.Engine.Obfuscate
{
    public class CrmObfuscateIntHandler : ICrmObfuscateHandler
    {
        private readonly ScramblerClient<int> intScramblerClient;

        public CrmObfuscateIntHandler()
        {
            intScramblerClient = new ScramblerClient<int>(new IntegerScrambler());
        }

        public bool CanHandle(Type type)
        {
            type.ThrowArgumentNullExceptionIfNull(nameof(type));

            return type.Equals(typeof(int));
        }

        public void HandleObfuscation(Entity entity, FieldToBeObfuscated field, IEntityMetadataCache metaData)
        {
            entity.ThrowArgumentNullExceptionIfNull(nameof(entity));
            field.ThrowArgumentNullExceptionIfNull(nameof(field));
            metaData.ThrowArgumentNullExceptionIfNull(nameof(metaData));

            // Get the min and maximum values for the field using the meta data cache
            IntegerAttributeMetadata intMetaData = (IntegerAttributeMetadata)metaData.GetAttribute(entity.LogicalName, field.FieldName);
            int min = intMetaData.MinValue.GetValueOrDefault(0);
            int max = intMetaData.MaxValue.GetValueOrDefault(10);
            entity[field.FieldName] = intScramblerClient.ExecuteScramble((int)entity[field.FieldName], min, max);
        }
    }
}