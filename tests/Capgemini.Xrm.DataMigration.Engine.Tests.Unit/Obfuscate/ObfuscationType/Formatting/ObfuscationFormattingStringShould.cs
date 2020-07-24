using Capgemini.DataMigration.Core.Model;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting.FormattingOptions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.Obfuscate.ObfuscationType.Formatting
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ObfuscationFormattingStringShould : UnitTestBase
    {
        protected ObfuscationFormattingString systemUnderTest;
        protected string originalValue;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
            Mock<FormattingOptionProcessor> mockFormattingOptionProcessor = OptionProcessorValidResponse();

            systemUnderTest = new ObfuscationFormattingString(mockFormattingOptionProcessor.Object);
            originalValue = "OriginalValue";
        }

        private static Mock<FormattingOptionProcessor> OptionProcessorValidResponse()
        {
            var mockFormattingOptionProcessor = new Mock<FormattingOptionProcessor>();
            mockFormattingOptionProcessor
                .Setup(a => a.GenerateFromLookup(It.IsAny<string>(), It.IsAny<ObfuscationFormatOption>()))
                .Returns("NewValue");
            return mockFormattingOptionProcessor;
        }

        [TestMethod]
        public void ReturnADifferentValueToTheOriginalValueFromTheMethodCreateFormattedValueWhenTheFirstValueMatchedTheOriginal()
        {
            Mock<FormattingOptionProcessor> mockFormattingOptionProcessor = OptionProcessorSequenceOfResponses();

            systemUnderTest = new ObfuscationFormattingString(mockFormattingOptionProcessor.Object);

            var newValue = systemUnderTest.CreateFormattedValue(originalValue, CreateFieldToBeObfuscatedValidObject(), CreateMetadataParamters());

            newValue.Should().NotBe(originalValue);
        }

        [TestMethod]
        public void ReturnADifferentValueToTheOriginalValueFromTheMethodCreateFormattedValue()
        {
            var newValue = systemUnderTest.CreateFormattedValue(originalValue, CreateFieldToBeObfuscatedValidObject(), CreateMetadataParamters());

            newValue.Should().NotBe(originalValue);
        }

        [TestMethod]
        public void ReturnADifferentValueToTheOriginalValueThatDoesNotExceedTheMaxLength()
        {
            var newValue = systemUnderTest.CreateFormattedValue(originalValue, CreateFieldToBeObfuscatedValidObject(), CreateMetadataParamters(5));

            newValue.Should().NotBe(originalValue);
            newValue.Length.Should().Be(5);
        }

        [TestMethod]
        public void ReturnArgumentNullExceptionIfFieldPassedAsNullToTheMethodCreateFormattedValue()
        {
            Action action = () => systemUnderTest.CreateFormattedValue(originalValue, null, CreateMetadataParamters());

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ReturnArgumentNullExceptionIfmetadataParametersPassedAsNullTheMethodCreateFormattedValue()
        {
            Action action = () => systemUnderTest.CreateFormattedValue(originalValue, CreateFieldToBeObfuscatedValidObject(), null);

            action.Should().Throw<ArgumentNullException>();
        }

        private Dictionary<string, object> CreateMetadataParamters(int maxLength = 100)
        {
            var metadataParameters = new Dictionary<string, object>();

            metadataParameters.Add("maxlength", maxLength);

            return metadataParameters;
        }

        private Mock<FormattingOptionProcessor> OptionProcessorSequenceOfResponses()
        {
            var mockFormattingOptionProcessor = new Mock<FormattingOptionProcessor>();

            var calls = 0;
            mockFormattingOptionProcessor
                .Setup(a => a.GenerateFromLookup(It.IsAny<string>(), It.IsAny<ObfuscationFormatOption>()))
                .Returns(() => AlterReturnValue(calls++));

            return mockFormattingOptionProcessor;
        }

        private string AlterReturnValue(int call)
        {
            if (call > 0)
            {
                return "NewValue";
            }

            return "OriginalValue";
        }

        private FieldToBeObfuscated CreateFieldToBeObfuscatedValidObject()
        {
            List<ObfuscationFormatOption> arguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> argumentsParams = new Dictionary<string, string>();
            argumentsParams.Add("filename", "FirstnameAndSurnames.csv");
            argumentsParams.Add("columnname", "latitude");

            arguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, argumentsParams));

            Dictionary<string, string> argumentsParamsString = new Dictionary<string, string>();
            argumentsParamsString.Add("length", "10");

            arguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomString, argumentsParamsString));

            Dictionary<string, string> argumentsParamsNumber = new Dictionary<string, string>();
            argumentsParamsNumber.Add("min", "0");
            argumentsParamsNumber.Add("max", "10");

            arguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomNumber, argumentsParamsNumber));


            return new FieldToBeObfuscated()
            {
                FieldName = "address1_line1",
                ObfuscationFormat = "{0}",
                ObfuscationFormatArgs = arguments
            };
        }

    }
}