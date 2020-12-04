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
    public class CrmObfuscateStringHandler : ICrmObfuscateHandler
    {
        private readonly ScramblerClient<string> strScramblerClient;
        private readonly IObfuscationFormattingType<string> formattingClient;

        public CrmObfuscateStringHandler()
        {
            this.strScramblerClient = new ScramblerClient<string>(new StringScrambler());
            this.formattingClient = new ObfuscationFormattingString(new FormattingOptionProcessor());
        }

        public CrmObfuscateStringHandler(IObfuscationFormattingType<string> formattingClient)
        {
            this.strScramblerClient = new ScramblerClient<string>(new StringScrambler());
            this.formattingClient = formattingClient;
        }

        public bool CanHandle(Type type)
        {
            type.ThrowArgumentNullExceptionIfNull(nameof(type));

            return type.Equals(typeof(string));
        }

        public void HandleObfuscation(Entity entity, FieldToBeObfuscated field, IEntityMetadataCache metaData)
        {
            entity.ThrowArgumentNullExceptionIfNull(nameof(entity));
            field.ThrowArgumentNullExceptionIfNull(nameof(field));
            metaData.ThrowArgumentNullExceptionIfNull(nameof(metaData));

            StringAttributeMetadata stringMetaData = (StringAttributeMetadata)metaData.GetAttribute(entity.LogicalName, field.FieldName);

            if (field.CanBeFormatted)
            {
                Dictionary<string, object> metadataParams = new Dictionary<string, object>();

                if (stringMetaData.MaxLength != null)
                {
                    metadataParams.Add("maxlength", stringMetaData.MaxLength);
                }

                entity[field.FieldName] = formattingClient.CreateFormattedValue((string)entity[field.FieldName], field, metadataParams);
                return;
            }

            entity[field.FieldName] = strScramblerClient.ExecuteScramble((string)entity[field.FieldName]);
        }
    }
}