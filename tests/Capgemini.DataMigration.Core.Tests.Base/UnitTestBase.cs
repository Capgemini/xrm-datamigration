using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataMigration.DataStore;
using Capgemini.DataMigration.Resiliency;
using Capgemini.Xrm.DataMigration.Config;
using Capgemini.Xrm.DataMigration.Core;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Moq;

namespace Capgemini.DataMigration.Core.Tests.Base
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public abstract class UnitTestBase
    {
        protected const string FilePrefix = "filePrefix";
        protected const string TestFetchXMLQuery = "<fetch><entity name=\"contact\"><attribute name=\"firstname\" /><attribute name=\"lastname\" /></entity></fetch>";

        public TestContext TestContext { get; set; }

        protected List<string> FetchXMlQueries { get; } = new List<string>()
            {
                @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""false"">
                  <entity name=""entity1"">
                    <attribute name=""ds_name"" />
                  </entity>
                </fetch>
                ",
                @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""false"">
                  <entity name=""entity2"">
                    <attribute name=""ds_name"" />
                  </entity>
                </fetch>
                "
            };

        protected List<EntityToBeObfuscated> EmptyFieldsToObfuscate { get; } = new List<EntityToBeObfuscated>();

        protected GenericDataMigrator<Entity, MigrationEntityWrapper<Entity>> SystemUnderTest { get; set; }

        protected Mock<ILogger> MockLogger { get; set; }

        protected Mock<IDataStoreReader<Entity, MigrationEntityWrapper<Entity>>> MockDataStoreReader { get; set; }

        protected Mock<IDataStoreWriter<Entity, MigrationEntityWrapper<Entity>>> MockDataStoreWriter { get; set; }

        protected Mock<IEntityProcessor<Entity, MigrationEntityWrapper<Entity>>> MockProcessor { get; set; }

        protected Mock<IEntityRepository> MockEntityRepo { get; set; }

        protected Mock<IEntityMetadataCache> MockEntityMetadataCache { get; set; }

        protected Mock<ICrmStoreReaderConfig> MockCrmStoreReaderConfig { get; set; }

        protected Mock<ICrmStoreWriterConfig> MockCrmStoreWriterConfig { get; set; }

        protected Mock<ICrmGenericImporterConfig> MockCrmGenericImporterConfig { get; set; }

        protected Mock<IFileStoreWriterConfig> MockFileStoreWriterConfig { get; set; }

        protected Mock<IOrganizationService> MockOrganizationService { get; set; }

        protected Mock<IRetryExecutor> MockRetryExecutor { get; set; }

        protected List<string> ExcludedFields { get; } = new List<string>();

        protected string TestResultFolder { get; set; }

        protected static void SetFieldValue(object input, string fieldName, object newValue)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var field = input.GetType().GetRuntimeFields().First(a => a.Name == fieldName);
            field.SetValue(input, newValue);
        }

        protected static T InjectMetaDataToResponse<T>(T response, EntityMetadata entityMetadata)
    where T : OrganizationResponse
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (response.Results == null)
            {
                response.Results = new ParameterCollection();
            }

            response.Results.Add(new KeyValuePair<string, object>("EntityMetadata", entityMetadata));

            return response;
        }

        protected static T InjectMetaDataToResponse<T>(T response, List<AttributeMetadata> attributes = null, List<OneToManyRelationshipMetadata> manyToOneRelationships = null, List<ManyToManyRelationshipMetadata> manyToManyRelationships = null, bool hasIntersectValue = false)
where T : OrganizationResponse
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var metaData = InitializeEntityMetadata(attributes, manyToOneRelationships, manyToManyRelationships, hasIntersectValue);

            if (response.Results == null)
            {
                response.Results = new ParameterCollection();
            }

            response.Results.Add(new KeyValuePair<string, object>("EntityMetadata", metaData));

            return response;
        }

        protected static EntityMetadata InitializeEntityMetadata(List<AttributeMetadata> attributes = null, List<OneToManyRelationshipMetadata> manyToOneRelationships = null, List<ManyToManyRelationshipMetadata> manyToManyRelationships = null, bool hasIntersectValue = false)
        {
            var metaData = new EntityMetadata();

            if (hasIntersectValue)
            {
                SetFieldValue(metaData, "_isIntersect", true);
            }

            if (manyToOneRelationships == null)
            {
                manyToOneRelationships = new List<OneToManyRelationshipMetadata>();
            }

            SetFieldValue(metaData, "_manyToOneRelationships", manyToOneRelationships.ToArray());

            if (manyToManyRelationships == null)
            {
                manyToManyRelationships = new List<ManyToManyRelationshipMetadata>();
            }

            SetFieldValue(metaData, "_manyToManyRelationships", manyToManyRelationships.ToArray());

            if (attributes == null)
            {
                attributes = new List<AttributeMetadata>();
            }

            SetFieldValue(metaData, "_attributes", attributes.ToArray());

            return metaData;
        }

        protected static void SafelyDeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Just bail out we don't care as this is a test artefact!
            }
            catch (IOException)
            {
                // Just bail out we don't care as this is a test artefact!
            }
        }

        protected static Entity CreateContact(string firstName, string lastName, EntityReference ownerId)
        {
            Entity contact = new Entity("contact")
            {
                Id = Guid.NewGuid()
            };
            contact.Attributes["firstname"] = firstName;
            contact.Attributes["lastname"] = lastName;
            contact.Attributes["ownerid"] = ownerId;

            return contact;
        }

        protected static List<EntityWrapper> PrepareEntities()
        {
            List<EntityWrapper> entities = new List<EntityWrapper>
            {
                new EntityWrapper(CreateContact("Firs Test 1", "Last Test 1", new EntityReference("systemuser", Guid.NewGuid())), false),
                new EntityWrapper(CreateContact("Firs Test 2", "Last Test 2", new EntityReference("systemuser", Guid.NewGuid())), false),
                new EntityWrapper(CreateContact("Firs Test 3", "Last Test 3", new EntityReference("team", Guid.NewGuid())), false),
                new EntityWrapper(CreateContact("Firs Test 4", "Last Test 4", new EntityReference("user", Guid.NewGuid())), false)
            };
            return entities;
        }

        protected static CrmSchemaConfiguration GetSchema(string extractedPath, string schemaFilename)
        {
            string schemaFilePath = Path.Combine(extractedPath, schemaFilename);

            return CrmSchemaConfiguration.ReadFromFile(schemaFilePath);
        }

        protected static CrmSchemaConfiguration GetSchema()
        {
            var schemaConfig = new CrmSchemaConfiguration();
            var entities = new List<CrmEntity>();
            var contact = new CrmEntity
            {
                Name = "contact",
                PrimaryIdField = "contactid"
            };

            var fields = new List<CrmField>
            {
                new CrmField { FieldName = "contactid", FieldType = "guid" },
                new CrmField { FieldName = "firstname", FieldType = "string" },
                new CrmField { FieldName = "lastname", FieldType = "string" },
                new CrmField { FieldName = "ownerid", FieldType = "entityreference", LookupType = "systemuser" }
            };
            contact.CrmFields.AddRange(fields);

            entities.Add(contact);
            schemaConfig.Entities.AddRange(entities);

            return schemaConfig;
        }

        protected void InitializeProperties()
        {
            MockOrganizationService = new Mock<IOrganizationService>();
            MockLogger = new Mock<ILogger>();
            MockDataStoreReader = new Mock<IDataStoreReader<Entity, MigrationEntityWrapper<Entity>>>();
            MockDataStoreWriter = new Mock<IDataStoreWriter<Entity, MigrationEntityWrapper<Entity>>>();
            MockProcessor = new Mock<IEntityProcessor<Entity, MigrationEntityWrapper<Entity>>>();
            MockEntityRepo = new Mock<IEntityRepository>();
            MockCrmStoreReaderConfig = new Mock<ICrmStoreReaderConfig>();
            MockCrmStoreWriterConfig = new Mock<ICrmStoreWriterConfig>();
            MockEntityMetadataCache = new Mock<IEntityMetadataCache>();
            MockCrmGenericImporterConfig = new Mock<ICrmGenericImporterConfig>();
            MockFileStoreWriterConfig = new Mock<IFileStoreWriterConfig>();
            MockRetryExecutor = new Mock<IRetryExecutor>();
            TestResultFolder = TestContext.TestResultsDirectory;
        }
    }
}