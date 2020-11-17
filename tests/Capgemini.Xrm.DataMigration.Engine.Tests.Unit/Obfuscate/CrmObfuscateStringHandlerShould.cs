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
    public class CrmObfuscateStringHandlerShould : UnitTestBase
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
            var mockFormattingClient = new Mock<IObfuscationFormattingType<string>>();
            mockFormattingClient.Setup(a => a.CreateFormattedValue(It.IsAny<string>(), It.IsAny<FieldToBeObfuscated>(), It.IsAny<Dictionary<string, object>>()))
                .Returns("126 New Close");
            var systemUnderTest = new CrmObfuscateStringHandler(mockFormattingClient.Object);

            InitializeEntityMetadata();
            MockEntityMetadataCache
                .Setup(cache => cache.GetAttribute("contact", "address1_line1"))
                .Returns(new StringAttributeMetadata()
                {
                    MaxLength = 200
                });

            string originalValue = "1 Main Road";

            Entity entity = new Entity("contact");
            entity.Attributes.Add("address1_line1", originalValue);

            List<ObfuscationFormatOption> arguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> argumentsParams = new Dictionary<string, string>
            {
                { "filename", "test.csv" },
                { "columnname", "street" }
            };

            arguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, argumentsParams));

            var fieldToBeObfuscated = new FieldToBeObfuscated()
            {
                FieldName = "address1_line1",
                ObfuscationFormat = "{0}"
            };
            fieldToBeObfuscated.ObfuscationFormatArgs.AddRange(arguments);

            // Act
            systemUnderTest.HandleObfuscation(entity, fieldToBeObfuscated, MockEntityMetadataCache.Object);

            string newValue = entity["address1_line1"].ToString();

            newValue.Should().NotBe(originalValue);
        }
    }
}