using System;
using System.Linq;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataScrambler;
using Capgemini.DataScrambler.Scramblers;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace Capgemini.Xrm.DataMigration.Engine.Obfuscate
{
    public class CrmObfuscateStringHandler : ICrmObfuscateHandler
    {
        private ScramblerClient<string> strScramblerClient;
        private IObfuscationFormattingType<string> formattingClient;

        public CrmObfuscateStringHandler()
        {
            this.strScramblerClient = new ScramblerClient<string>(new StringScrambler());
            this.formattingClient = new ObfuscationFormattingString();
        }

        public bool CanHandle(Type type)
        {
            return type.Equals(typeof(string));
        }

        public void HandleObfuscation(Entity entity, FieldToBeObfuscated field, IEntityMetadataCache metaData)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            if (metaData == null)
            {
                throw new ArgumentNullException(nameof(metaData));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            StringAttributeMetadata stringMetaData = (StringAttributeMetadata)metaData.GetAttribute(entity.LogicalName, field.FieldName);

            if (field.CanBeFormatted)
            {
                entity[field.FieldName] = formattingClient.CreateFormattedValue((string)entity[field.FieldName], field);
            }
            else
            {
                entity[field.FieldName] = strScramblerClient.ExecuteScramble((string)entity[field.FieldName]);
            }
        }
    }
}
