﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.DataStore;
using CsvHelper;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.FileStore.Helpers
{
    internal class ReadCsvHelper
    {
        private readonly CrmSchemaConfiguration schemaConfig;

        public ReadCsvHelper(CrmSchemaConfiguration schemaConfig)
        {
            this.schemaConfig = schemaConfig;
        }

        public List<EntityWrapper> ReadFromFile(string fileName, string entityName)
        {
            List<EntityWrapper> store = new List<EntityWrapper>();
            string headerLine = File.ReadLines(fileName).FirstOrDefault();
            List<string> header = headerLine.Split(',').ToList();

            using (TextReader tr = File.OpenText(fileName))
            {
                using (var reader = new CsvReader(tr))
                {
                    // Ignore Header - empty read
                    reader.Read();

                    while (reader.Read())
                    {
                        Entity ent = new Entity(entityName);
                        int idx = 0;
                        foreach (var item in header)
                        {
                            if (item.StartsWith("map.", StringComparison.Ordinal))
                            {
                                ReadAliassedValue(reader, ent, idx, item);
                            }
                            else
                            {
                                ReadCsvValue(entityName, reader, ent, idx, item);
                            }

                            idx++;
                        }

                        bool isManyToMany = schemaConfig.Entities.Any(p => p.CrmRelationships.Any(r => r.RelationshipName == entityName));
                        store.Add(new EntityWrapper(ent, isManyToMany));
                    }
                }
            }

            return store;
        }

        private static void ReadAliassedValue(CsvReader reader, Entity entity, int idx, string aliasAttrName)
        {
            List<string> aliases = aliasAttrName.Split('.').ToList();
            string entName = aliases[2];
            string attrName = aliases.Last();

            if (reader.TryGetField(typeof(string), idx, out object obj))
            {
                if (entName == "isRootBU")
                {
                    entName = "businessunit";
                }

                AliasedValue alias = new AliasedValue(entName, attrName, obj);
                entity.Attributes[aliasAttrName] = alias;
            }
        }

        private void ReadCsvValue(string entityName, CsvReader reader, Entity entity, int idx, string attrName)
        {
            string fieldName = attrName;

            var field = schemaConfig.Entities.Single(p => p.Name == entityName).CrmFields.FirstOrDefault(p => p.FieldName == fieldName);

            if (field != null)
            {
                if (field.PrimaryKey)
                {
                    entity.Id = reader.GetField<Guid>(idx);
                }
                else
                {
                    if (reader.TryGetField(EntityConverterHelper.GetAttributeTypeForCsv(field.FieldType), idx, out object obj))
                    {
                        object val = EntityConverterHelper.GetAttributeValueFromCsv(field.FieldType, field.LookupType, obj);

                        if (val != null)
                        {
                            entity.Attributes[attrName] = val;
                        }
                    }
                }
            }
        }
    }
}