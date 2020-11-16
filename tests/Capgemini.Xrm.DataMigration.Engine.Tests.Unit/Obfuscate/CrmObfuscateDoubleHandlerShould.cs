using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Moq;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.Obfuscate
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmObfuscateDoubleHandlerShould : UnitTestBase
    {
        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
        }

        [TestMethod]
        public void ReturnAFormattedValue()
        {
            // Arrange
            var mockFormattingClient = new Mock<IObfuscationFormattingType<double>>();
            mockFormattingClient.Setup(a => a.CreateFormattedValue(It.IsAny<double>(), It.IsAny<FieldToBeObfuscated>(), It.IsAny<Dictionary<string, object>>()))
                .Returns(49.123);
            CrmObfuscateDoubleHandler systemUnderTest = new CrmObfuscateDoubleHandler(mockFormattingClient.Object);

            InitializeEntityMetadata();
            MockEntityMetadataCache
                .Setup(cache => cache.GetAttribute("contact", "address1_latitude"))
                .Returns(new DoubleAttributeMetadata());

            double latitudeBefore = 51.5178737;

            Entity entity = new Entity("contact");
            entity.Attributes.Add("address1_latitude", latitudeBefore);

            List<ObfuscationFormatOption> arguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> argumentsParams = new Dictionary<string, string>();
            argumentsParams.Add("filename", "FirstnameAndSurnames.csv");
            argumentsParams.Add("columnname", "latitude");

            arguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, argumentsParams));

            var fieldToBeObfuscated = new FieldToBeObfuscated()
            {
                FieldName = "address1_latitude",
                ObfuscationFormat = "{0}"
            };
            fieldToBeObfuscated.ObfuscationFormatArgs.AddRange(arguments);

            // Act
            systemUnderTest.HandleObfuscation(entity, fieldToBeObfuscated, MockEntityMetadataCache.Object);

            double latitudeAfter = (double)entity["address1_latitude"];

            latitudeAfter.Should().NotBe(latitudeBefore);
        }
    }
}