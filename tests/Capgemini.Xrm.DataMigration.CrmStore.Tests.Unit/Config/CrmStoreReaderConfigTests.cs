﻿using System.Collections.Generic;
using Capgemini.DataMigration.Core.Tests.Base;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.CrmStore.Config.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmStoreReaderConfigTests : UnitTestBase
    {
        private readonly List<string> fetchXMlQueries = new List<string>()
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

        private CrmStoreReaderConfig systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            systemUnderTest = new CrmStoreReaderConfig(fetchXMlQueries);
        }

        [TestMethod]
        public void CrmStoreReaderConfig()
        {
            var actual = new CrmStoreReaderConfig(fetchXMlQueries);

            actual.BatchSize.Should().Be(500);
            actual.OneEntityPerBatch.Should().Be(true);
            actual.PageSize.Should().Be(500);
            actual.TopCount.Should().Be(500);
        }

        [TestMethod]
        public void GetFetchXMLQueries()
        {
            var actual = systemUnderTest.GetFetchXMLQueries();

            actual.Count.Should().Be(2);
        }
    }
}