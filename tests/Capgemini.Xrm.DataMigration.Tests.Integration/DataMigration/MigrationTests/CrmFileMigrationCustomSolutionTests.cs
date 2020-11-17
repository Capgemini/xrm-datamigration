using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Capgemini.DataMigration.Core;
using Capgemini.DataMigration.Resiliency.Polly;
using Capgemini.Xrm.DataMigration.CrmStore.Config;
using Capgemini.Xrm.DataMigration.DataStore;
using Capgemini.Xrm.DataMigration.Extensions;
using Capgemini.Xrm.DataMigration.FileStore.DataStore;
using Capgemini.Xrm.DataMigration.IntegrationTests.DataMigration.MigrationTests;
using Capgemini.Xrm.DataMigration.Repositories;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Capgemini.Xrm.DataMigration.IntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmFileMigrationCustomSolutionTests : CrmFileMigrationBaseTest
    {
        public CrmFileMigrationCustomSolutionTests()
            : base(
            "TestData\\ImportSchemas\\CustomSolutionSchema",
            "CustomSolutionSchema.xml",
            "CrmConfiguration",
            ConnectionHelper.GetOrganizationalServiceSource(),
            ConnectionHelper.GetOrganizationalServiceTarget())
        {
            CreateSourceData();
        }

        [TestMethod]
        public void RandomizeGuidsInFolder()
        {
            string folderPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            string schemaFolderPath = Path.Combine(folderPath, "ImportSchemas\\CustomSolutionSchema");

            string extractedDataPath = Path.Combine(schemaFolderPath, "ExtractedData");
            string randomisedPath = Path.Combine(extractedDataPath, "..\\RandomGuids");

            Directory.CreateDirectory(randomisedPath);
            foreach (var file in Directory.EnumerateFiles(randomisedPath))
            {
                DeleteFileIfExits(file);
            }

            DataFileStoreReader dsfr = new DataFileStoreReader(new ConsoleLogger(), "ExportedData", extractedDataPath);

            List<EntityWrapper> data = null;

            FluentActions.Invoking(() => data = dsfr.ReadBatchDataFromStore())
                    .Should()
                    .NotThrow();

            while (data != null && data.Count > 0)
            {
                foreach (var record in data)
                {
                    record.OriginalEntity.Id = Guid.NewGuid();

                    var attribs = data.SelectMany(d => d.OriginalEntity.Attributes).Where(a => a.Value is EntityReference || a.Value is Guid).ToList();
                    foreach (var a in attribs)
                    {
                        if (a.Value is EntityReference temp)
                        {
                            ((EntityReference)a.Value).Id = Guid.NewGuid();
                        }
                        else if (a.Value is Guid)
                        {
                            record.OriginalEntity[a.Key] = Guid.NewGuid();
                        }
                    }
                }

                new DataFileStoreWriter(new ConsoleLogger(), "ExportedData", randomisedPath).SaveBatchDataToStore(data);
                data = dsfr.ReadBatchDataFromStore();

                FluentActions.Invoking(() => data = dsfr.ReadBatchDataFromStore())
                    .Should()
                    .NotThrow();
            }

            foreach (var file in Directory.EnumerateFiles(randomisedPath))
            {
                var traget = Path.Combine(extractedDataPath, Path.GetFileName(file));
                DeleteFileIfExits(traget);
                File.Move(file, traget);
            }
        }

        protected override CrmExporterConfig GetExporterConfig()
        {
            var exportConfig = base.GetExporterConfig();

            exportConfig.CrmMigrationToolSchemaFilters.Add("team", "<filter><condition attribute=\"name\" operator=\"eq\" value=\"KrissTestt\" /></filter>");
            exportConfig.CrmMigrationToolSchemaFilters.Add("queue", "<filter><condition attribute=\"name\" operator=\"like\" value=\"%KrissTestt%\" /></filter>");
            exportConfig.CrmMigrationToolSchemaFilters.Add("subject", "<filter><condition attribute=\"title\" operator=\"ne\" value=\"Default Subject\" /></filter>");
            exportConfig.CrmMigrationToolSchemaFilters.Add("businessunit", "<filter><condition attribute=\"parentbusinessunitid\" operator=\"not-null\"/></filter>");

            exportConfig.LookupMapping.Add("new_new_mydata_new_myrefdata", new Dictionary<string, List<string>>());
            exportConfig.LookupMapping["new_new_mydata_new_myrefdata"].Add("new_mydataid", new List<string> { "new_name" });
            exportConfig.LookupMapping["new_new_mydata_new_myrefdata"].Add("new_myrefdataid", new List<string> { "new_name" });

            exportConfig.LookupMapping.Add("businessunit", new Dictionary<string, List<string>>());
            exportConfig.LookupMapping["businessunit"].Add("businessunitid", new List<string> { "name", "divisionname" });

            exportConfig.LookupMapping.Add("new_mydata", new Dictionary<string, List<string>>());
            exportConfig.LookupMapping["new_mydata"].Add("ownerid", new List<string> { "fullname" });
            exportConfig.LookupMapping["new_mydata"].Add("new_parentbuid", new List<string> { "name", "divisionname" });
            exportConfig.LookupMapping["new_mydata"].Add("new_bulookupid", new List<string> { "name", "divisionname" });
            exportConfig.LookupMapping["new_mydata"].Add("new_myrefdataid", new List<string> { "new_name" });
            exportConfig.LookupMapping["new_mydata"].Add("new_datacircular", new List<string> { "new_name" });

            exportConfig.LookupMapping.Add("new_refobentities", new Dictionary<string, List<string>>());
            exportConfig.LookupMapping["new_refobentities"].Add("new_user", new List<string> { "fullname" });
            exportConfig.LookupMapping["new_refobentities"].Add("new_seciurityroleid", new List<string> { "name" });
            exportConfig.LookupMapping["new_refobentities"].Add("new_subjectid", new List<string> { "title" });
            exportConfig.LookupMapping["new_refobentities"].Add("new_teamid", new List<string> { "name" });
            exportConfig.LookupMapping["new_refobentities"].Add("new_queueid", new List<string> { "name" });

            exportConfig.LookupMapping.Add("new_new_refobentities_queue", new Dictionary<string, List<string>>());
            exportConfig.LookupMapping["new_new_refobentities_queue"].Add("new_refobentitiesid", new List<string> { "new_name" });
            exportConfig.LookupMapping["new_new_refobentities_queue"].Add("queueid", new List<string> { "name" });

            exportConfig.LookupMapping.Add("new_new_refobentities_team", new Dictionary<string, List<string>>());
            exportConfig.LookupMapping["new_new_refobentities_team"].Add("new_refobentitiesid", new List<string> { "new_name" });
            exportConfig.LookupMapping["new_new_refobentities_team"].Add("teamid", new List<string> { "name" });

            exportConfig.LookupMapping.Add("team", new Dictionary<string, List<string>>());
            exportConfig.LookupMapping["team"].Add("teamid", new List<string> { "name" });
            exportConfig.LookupMapping["team"].Add("businessunitid", new List<string> { "name" });

            exportConfig.LookupMapping.Add("queue", new Dictionary<string, List<string>>());
            exportConfig.LookupMapping["queue"].Add("queueid", new List<string> { "name" });

            return exportConfig;
        }

        private static void DeleteFileIfExits(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        private void CreateSourceData()
        {
            Guid lastMyDataId = Guid.Empty;

            var buId = CreateIfNotExist_businessunit("Test BU");
            List<Guid> refDatas = new List<Guid>();

            if (buId != Guid.Empty)
            {
                for (int i = 0; i < 10; i++)
                {
                    var refDataId = CreateIfNotExist_new_myrefdata($"Test ref data {i}");
                    if (refDataId != Guid.Empty)
                    {
                        lastMyDataId = CreateIfNotExist_new_mydata($"Test my data {i}", lastMyDataId, refDataId, buId, refDatas);
                        refDatas.Add(refDataId);
                    }
                }
            }
        }

        private Guid CreateIfNotExist_businessunit(string name)
        {
            IOrganizationService service = SourceService;
            var repo = new EntityRepository(service, new ServiceRetryExecutor());

            var records = service.GetEntitiesByColumn("businessunit", "name", name);

            if (records.Entities.Count == 0)
            {
                var ent = new Entity("businessunit");
                ent.Attributes["name"] = name;
                ent.Attributes["parentbusinessunitid"] = new EntityReference("businessunit", repo.GetParentBuId());
                return service.Create(ent);
            }

            return Guid.Empty;
        }

        private Guid CreateIfNotExist_new_myrefdata(string name)
        {
            IOrganizationService service = SourceService;

            var records = service.GetEntitiesByColumn("new_myrefdata", "new_name", name);

            if (records.Entities.Count == 0)
            {
                var ent = new Entity("new_myrefdata");
                ent.Attributes["new_name"] = name;
                return service.Create(ent);
            }

            return Guid.Empty;
        }

        private Guid CreateIfNotExist_new_mydata(string name, Guid new_datacircularId, Guid new_myrefdataId, Guid buId, List<Guid> refDatas)
        {
            IOrganizationService service = SourceService;

            var repo = new EntityRepository(service, new ServiceRetryExecutor());
            Guid retValue = Guid.Empty;

            var records = service.GetEntitiesByColumn("new_mydata", "new_name", name);

            if (records.Entities.Count == 0)
            {
                var ent = new Entity("new_mydata");
                ent.Attributes["new_name"] = name;
                ent.Attributes["new_bulookupid"] = new EntityReference("businessunit", buId);
                ent.Attributes["new_parentbuid"] = new EntityReference("businessunit", repo.GetParentBuId());

                if (new_datacircularId != Guid.Empty)
                {
                    ent.Attributes["new_datacircular"] = new EntityReference("new_mydata", new_datacircularId);
                }

                if (new_myrefdataId != Guid.Empty)
                {
                    ent.Attributes["new_myrefdataid"] = new EntityReference("new_myrefdata", new_myrefdataId);
                }

                retValue = service.Create(ent);
            }

            if (refDatas != null && retValue != Guid.Empty)
            {
                List<EntityWrapper> ents = new List<EntityWrapper>();
                foreach (var refDataId in refDatas)
                {
                    var manyToManyEnt = new Entity("new_new_mydata_new_myrefdata");
                    manyToManyEnt.Attributes["new_mydataid"] = retValue;
                    manyToManyEnt.Attributes["new_myrefdataid"] = refDataId;
                    var wrap = new EntityWrapper(manyToManyEnt, true);
                    ents.Add(wrap);
                }

                if (ents.Any())
                {
                    repo.AssociateManyToManyEntity(ents);
                }
            }

            return retValue;
        }
    }
}