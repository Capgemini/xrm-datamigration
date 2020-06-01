using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Capgemini.DataMigration.Core.Extensions;
using Capgemini.DataMigration.Exceptions;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.FileStore.Model;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Capgemini.Xrm.DataMigration.FileStore.Helpers
{
    public static class EntityConverterHelper
    {
        /// <summary>
        /// Converts CrmEntityStore into Entity.
        /// </summary>
        /// <param name="entitylist">entitylist.</param>
        /// <returns>value.</returns>
        public static List<EntityWrapper> ConvertEntities(List<CrmEntityStore> entitylist)
        {
            var ec = entitylist.Select(e => new EntityWrapper(
                        new Entity(e.LogicalName, e.Id)
                        {
                            Attributes = ConvertAttributes(e.Attributes)
                        }, e.IsManyToMany)).ToList();

            return ec;
        }

        /// <summary>
        /// Converts CrmAttributeStore into AttributeCollection.
        /// </summary>
        /// <param name="attributes">attributes.</param>
        /// <returns>value.</returns>
        public static AttributeCollection ConvertAttributes(List<CrmAttributeStore> attributes)
        {
            attributes.ThrowArgumentNullExceptionIfNull(nameof(attributes));

            var atrCollection = new AttributeCollection();

            foreach (CrmAttributeStore attribute in attributes)
            {
                object attributeValue = null;

                switch (attribute.AttributeType)
                {
                    case "System.Guid":
                        attributeValue = new Guid((string)attribute.AttributeValue);
                        break;

                    case "System.Decimal":
                        attributeValue = Convert.ToDecimal(attribute.AttributeValue, CultureInfo.InvariantCulture);
                        break;

                    case "System.Double":
                        attributeValue = Convert.ToDouble(attribute.AttributeValue, CultureInfo.InvariantCulture);
                        break;

                    case "System.Int32":
                        attributeValue = Convert.ToInt32(attribute.AttributeValue, CultureInfo.InvariantCulture);
                        break;

                    case "Microsoft.Xrm.Sdk.EntityReference":
                        var entRef = (JObject)attribute.AttributeValue;
                        var lookup = new EntityReference((string)entRef["LogicalName"], (Guid)entRef["Id"]);
                        attributeValue = lookup;
                        break;

                    case "Microsoft.Xrm.Sdk.OptionSetValue":
                        var optSetValue = (JObject)attribute.AttributeValue;
                        attributeValue = new OptionSetValue { Value = (int)optSetValue["Value"] };
                        break;

                    case "Microsoft.Xrm.Sdk.OptionSetValueCollection":
                        var optionSetValues = (JArray)attribute.AttributeValue;
                        attributeValue = new OptionSetValueCollection(optionSetValues.Select(x => new OptionSetValue { Value = (int)x["Value"] }).ToList());
                        break;

                    case "Microsoft.Xrm.Sdk.Money":
                        var money = (JObject)attribute.AttributeValue;
                        attributeValue = new Money { Value = (decimal)money["Value"] };
                        break;

                    case "Microsoft.Xrm.Sdk.EntityCollection":
                        var entCollection = (JArray)attribute.AttributeValue;

                        var childEntities = entCollection.Children()
                            .Select(child => JsonConvert.DeserializeObject<CrmEntityStore>(child.ToString())).ToList();

                        attributeValue = new EntityCollection(ConvertEntities(childEntities).Select(p => p.OriginalEntity).ToList());
                        break;

                    case "System.Byte[]":
                        attributeValue = Convert.FromBase64String(attribute.AttributeValue.ToString());
                        break;

                    case "Microsoft.Xrm.Sdk.AliasedValue":
                        attributeValue = ProcessAliasedValue(attribute, attributeValue);
                        break;

                    default:
                        attributeValue = attribute.AttributeValue;
                        break;
                }

                atrCollection.Add(new KeyValuePair<string, object>(attribute.AttributeName, attributeValue));
            }

            return atrCollection;
        }

        public static object GetAttributeValueForCsv(CrmAttributeStore attribute)
        {
            attribute.ThrowArgumentNullExceptionIfNull(nameof(attribute));

            if (attribute.AttributeValue == null)
            {
                return null;
            }

            object attributeValue = string.Empty;

            switch (attribute.AttributeType)
            {
                case "Microsoft.Xrm.Sdk.EntityReference":
                    var entRef = (EntityReference)attribute.AttributeValue;
                    attributeValue = entRef.Id;
                    break;

                case "Microsoft.Xrm.Sdk.OptionSetValue":
                    var optSetValue = (OptionSetValue)attribute.AttributeValue;
                    attributeValue = optSetValue.Value;
                    break;

                case "Microsoft.Xrm.Sdk.OptionSetValueCollection":
                    var optionSetValues = (OptionSetValueCollection)attribute.AttributeValue;

                    var s = new StringBuilder();

                    optionSetValues.ToList().ForEach(x =>
                    {
                        if (s.Length == 0)
                        {
                            s.Append(x.Value.ToString(CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            s.Append("|" + x.Value.ToString(CultureInfo.InvariantCulture));
                        }
                    });

                    attributeValue = s.ToString();
                    break;

                case "Microsoft.Xrm.Sdk.Money":
                    var money = (Money)attribute.AttributeValue;
                    attributeValue = money.Value;
                    break;

                case "System.Byte[]":
                case "Microsoft.Xrm.Sdk.EntityCollection":
                    throw new ConfigurationException($"Not supported attribute type {attribute.AttributeType}");

                case "Microsoft.Xrm.Sdk.AliasedValue":
                    // at the monent alsiaed value would be always entity ref - if other types will be used, we need some proper logic here!
                    var aliasedValue = (AliasedValue)attribute.AttributeValue;
                    if (aliasedValue.Value is EntityReference entref)
                    {
                        attributeValue = entref.Name;
                    }
                    else if (aliasedValue.Value is OptionSetValue optSet)
                    {
                        attributeValue = optSet.Value;
                    }
                    else
                    {
                        attributeValue = aliasedValue.Value;
                    }

                    break;

                default:
                    attributeValue = attribute.AttributeValue;
                    break;
            }

            return attributeValue;
        }

        public static Type GetAttributeTypeForCsv(string itemType)
        {
            switch (itemType)
            {
                case "string":
                    return typeof(string);

                case "integer":
                    return typeof(int);

                case "guid":
                    return typeof(Guid);

                case "decimal":
                    return typeof(decimal);

                case "double":
                    return typeof(double);

                case "entityreference":
                    return typeof(Guid);

                case "optionsetvalue":
                    return typeof(int);

                case "optionsetvaluecollection":
                    return typeof(string);

                case "bool":
                    return typeof(bool);

                default:
                    throw new ConfigurationException($"Not supported item type {itemType}");
            }
        }

        public static object GetAttributeValueFromCsv(string itemType, string lookUpType, object input)
        {
            if (input == null)
            {
                return input;
            }

            switch (itemType)
            {
                case "string":
                    string strValue = Convert.ToString(input, CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(strValue))
                    {
                        return input;
                    }
                    else
                    {
                        return null;
                    }

                case "entityreference":
                    return new EntityReference(lookUpType, (Guid)input);

                case "optionsetvalue":
                    return new OptionSetValue((int)input);

                case "optionsetvaluecollection":
                    string str = Convert.ToString(input, CultureInfo.InvariantCulture);
                    return new OptionSetValueCollection(str.Split('|').Select(x => new OptionSetValue(int.Parse(x, CultureInfo.InvariantCulture))).ToList());

                default:
                    return input;
            }
        }

        private static object ProcessAliasedValue(CrmAttributeStore attribute, object attributeValue)
        {
            var aliasedValue = (JObject)attribute.AttributeValue;
            if (aliasedValue["Value"].Any())
            {
                try
                {
                    var entValue = aliasedValue["Value"];

                    if (entValue["Name"] != null)
                    {
                        var aliasedValueLookup = new AliasedValue((string)aliasedValue["EntityLogicalName"], (string)aliasedValue["AttributeLogicalName"], (string)entValue["Name"]);
                        attributeValue = aliasedValueLookup;
                    }
                    else if (entValue["Value"] != null)
                    {
                        var aliasedValueLookup = new AliasedValue((string)aliasedValue["EntityLogicalName"], (string)aliasedValue["AttributeLogicalName"], (string)entValue["Value"]);
                        attributeValue = aliasedValueLookup;
                    }
                }
                catch (Exception ex)
                {
                    throw new ConfigurationException($"Unsupported type used for alias {(string)aliasedValue["EntityLogicalName"]}, only EntityReference and string are supported, error:{ex.ToString()}", ex);
                }
            }
            else
            {
                var aliasedValueLookup = new AliasedValue((string)aliasedValue["EntityLogicalName"], (string)aliasedValue["AttributeLogicalName"], (string)aliasedValue["Value"]);
                attributeValue = aliasedValueLookup;
            }

            return attributeValue;
        }
    }
}