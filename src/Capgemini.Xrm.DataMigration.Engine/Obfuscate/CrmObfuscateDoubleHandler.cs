using System;
using System.Collections.Generic;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataScrambler;
using Capgemini.DataScrambler.Scramblers;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting.FormattingOptions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace Capgemini.Xrm.DataMigration.Engine.Obfuscate
{
    public class CrmObfuscateDoubleHandler : ICrmObfuscateHandler
    {
        private readonly ScramblerClient<double> doubleScramblerClient;
        private readonly IObfuscationFormattingType<double> formattingClient;

        public CrmObfuscateDoubleHandler()
        {
            this.doubleScramblerClient = new ScramblerClient<double>(new DoubleScrambler());
            this.formattingClient = new ObfuscationFormattingDouble(new FormattingOptionProcessor());
        }

        public CrmObfuscateDoubleHandler(IObfuscationFormattingType<double> formattingClient)
        {
            this.doubleScramblerClient = new ScramblerClient<double>(new DoubleScrambler());
            this.formattingClient = formattingClient;
        }

        public bool CanHandle(Type type)
        {
            type.ThrowArgumentNullExceptionIfNull(nameof(type));

            return type.Equals(typeof(double));
        }

        public void HandleObfuscation(Entity entity, FieldToBeObfuscated field, IEntityMetadataCache metaData)
        {
            entity.ThrowArgumentNullExceptionIfNull(nameof(entity));
            field.ThrowArgumentNullExceptionIfNull(nameof(field));
            metaData.ThrowArgumentNullExceptionIfNull(nameof(metaData));

            // Get the min and maximum values for the field using the meta data cache
            DoubleAttributeMetadata doubleMetaData = (DoubleAttributeMetadata)metaData.GetAttribute(entity.LogicalName, field.FieldName);

            int min = (int)doubleMetaData.MinValue.GetValueOrDefault(0);
            int max = (int)doubleMetaData.MaxValue.GetValueOrDefault(10);

            if (field.CanBeFormatted)
            {
                Dictionary<string, object> metadataParameters = new Dictionary<string, object>
                {
                    { "min", min },
                    { "max", max }
                };

                entity[field.FieldName] = formattingClient.CreateFormattedValue((double)entity[field.FieldName], field, metadataParameters);
                return;
            }

            entity[field.FieldName] = doubleScramblerClient.ExecuteScramble((double)entity[field.FieldName], min, max);
        }
    }
}