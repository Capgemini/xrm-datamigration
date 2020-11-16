using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataMigration.Core.Tests.Base;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting.FormattingOptions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.Obfuscate.ObfuscationType.Formatting
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ObfuscationFormattingDoubleShould : UnitTestBase
    {
        private ObfuscationFormattingDouble systemUnderTest;
        private double originalValue;
        private double returnValue;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
            Mock<FormattingOptionProcessor> mockFormattingOptionProcessor = OptionProcessorValidResponse();

            systemUnderTest = new ObfuscationFormattingDouble(mockFormattingOptionProcessor.Object);
            originalValue = 1.1111;
            returnValue = originalValue;
        }

        [TestMethod]
        public void ReturnADifferentValueToTheOriginalValueFromTheMethodCreateFormattedValue()
        {
            var newValue = systemUnderTest.CreateFormattedValue(originalValue, CreateFieldToBeObfuscatedValidObject(), CreateValidMetadataParamters());

            newValue.Should().NotBe(originalValue);
        }

        [TestMethod]
        public void ReturnADifferentValueToTheOriginalValueFromTheMethodCreateFormattedValueWhenTheFirstValueMatchedTheOriginal()
        {
            Mock<FormattingOptionProcessor> mockFormattingOptionProcessor = OptionProcessorSequenceOfResponses();

            systemUnderTest = new ObfuscationFormattingDouble(mockFormattingOptionProcessor.Object);

            var newValue = systemUnderTest.CreateFormattedValue(originalValue, CreateFieldToBeObfuscatedValidObject(), CreateValidMetadataParamters());

            newValue.Should().NotBe(originalValue);
        }

        [TestMethod]
        public void ThrowNotImplemetedExceptionIfAnArgumentOtherThanTypeLookupIsPassedToTheMethodCreateFormattedValue()
        {
            Action action = () => systemUnderTest.CreateFormattedValue(originalValue, CreateFieldToBeObfuscatedInvalidObject(), CreateValidMetadataParamters());

            action.Should().Throw<NotImplementedException>();
        }

        [TestMethod]
        public void ReturnArgumentNullExceptionIfFieldPassedAsNullToTheMethodCreateFormattedValue()
        {
            Action action = () => systemUnderTest.CreateFormattedValue(originalValue, null, CreateValidMetadataParamters());

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ReturnArgumentNullExceptionIfmetadataParametersPassedAsNullTheMethodCreateFormattedValue()
        {
            Action action = () => systemUnderTest.CreateFormattedValue(originalValue, CreateFieldToBeObfuscatedInvalidObject(), null);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ThrowExceptionIfMoreThanObfuscationFormatArgIsPassed()
        {
            Action action = () => systemUnderTest.CreateFormattedValue(originalValue, CreateFieldToBeObfuscatedObjectWithMultipleArguments(), CreateValidMetadataParamters());

            action.Should().Throw<Exception>();
        }

        [TestMethod]
        public void ThrowInvalidCastExceptionIfTheLookupValueReturnedCannotBeCastToADouble()
        {
            Mock<FormattingOptionProcessor> mockFormattingOptionProcessor = OptionProcessorInvalidResponse();

            ObfuscationFormattingDouble localSystemUnderTest = new ObfuscationFormattingDouble(mockFormattingOptionProcessor.Object);

            Action action = () => localSystemUnderTest.CreateFormattedValue(originalValue, CreateFieldToBeObfuscatedValidObject(), CreateValidMetadataParamters());

            action.Should().Throw<InvalidCastException>();
        }

        [TestMethod]
        public void ReturnTrueIfTheReplacementValueIsOutsideTheAlllowedRange()
        {
            var isValid = ObfuscationFormattingDouble.ReplacementIsValid(1, CreateValidMetadataParamters());

            isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ReturnFalseIfTheReplacementValueIsHigherThanTheMaxConstraint()
        {
            var isValid = ObfuscationFormattingDouble.ReplacementIsValid(1000, CreateValidMetadataParamters());

            isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ReturnFalseIfTheReplacementValueIsLowerThanTheMaxConstraint()
        {
            var isValid = ObfuscationFormattingDouble.ReplacementIsValid(1000, CreateValidMetadataParamters(2000, 3000));

            isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ReturnFalseIfTheNewValueIsOutsideTheValidRange()
        {
            bool isValid = ObfuscationFormattingDouble.ReplacementIsValid(20, CreateValidMetadataParamters(-10, 10));

            isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ReturnTrueIfTheNewValueIsInsideTheValidRange()
        {
            bool isValid = ObfuscationFormattingDouble.ReplacementIsValid(20, CreateValidMetadataParamters());

            isValid.Should().BeTrue();
        }

        [TestMethod]
        public void HandleMetadataParametersBeingPassedAsNull()
        {
            Action action = () => ObfuscationFormattingDouble.ReplacementIsValid(5, null);

            action.Should().NotThrow();
        }

        private static string AlterReturnValue(int call)
        {
            if (call > 0)
            {
                return "2.222";
            }

            return "1.1111";
        }

        private Mock<FormattingOptionProcessor> OptionProcessorValidResponse()
        {
            var mockFormattingOptionProcessor = new Mock<FormattingOptionProcessor>();
            mockFormattingOptionProcessor
                .Setup(a => a.GenerateFromLookup(It.IsAny<string>(), It.IsAny<ObfuscationFormatOption>()))
                .Returns("2.222");
            return mockFormattingOptionProcessor;
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

        private Mock<FormattingOptionProcessor> OptionProcessorInvalidResponse()
        {
            var mockFormattingOptionProcessor = new Mock<FormattingOptionProcessor>();
            mockFormattingOptionProcessor
                .Setup(a => a.GenerateFromLookup(It.IsAny<string>(), It.IsAny<ObfuscationFormatOption>()))
                .Returns("string");
            return mockFormattingOptionProcessor;
        }

        private Dictionary<string, object> CreateValidMetadataParamters(int min = -100, int max = 100)
        {
            var metadataParameters = new Dictionary<string, object>();

            metadataParameters.Add("min", min);
            metadataParameters.Add("max", max);

            return metadataParameters;
        }

        private FieldToBeObfuscated CreateFieldToBeObfuscatedValidObject()
        {
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

            return fieldToBeObfuscated;
        }

        private FieldToBeObfuscated CreateFieldToBeObfuscatedObjectWithMultipleArguments()
        {
            List<ObfuscationFormatOption> arguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> argumentsParams = new Dictionary<string, string>();
            argumentsParams.Add("filename", "FirstnameAndSurnames.csv");
            argumentsParams.Add("columnname", "latitude");

            arguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, argumentsParams));
            arguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.Lookup, argumentsParams));

            var fieldToBeObfuscated = new FieldToBeObfuscated()
            {
                FieldName = "address1_latitude",
                ObfuscationFormat = "{0}"
            };
            fieldToBeObfuscated.ObfuscationFormatArgs.AddRange(arguments);

            return fieldToBeObfuscated;
        }

        private FieldToBeObfuscated CreateFieldToBeObfuscatedInvalidObject()
        {
            List<ObfuscationFormatOption> arguments = new List<ObfuscationFormatOption>();
            Dictionary<string, string> argumentsParams = new Dictionary<string, string>();
            argumentsParams.Add("min", "-10.000");
            argumentsParams.Add("max", "60.000");

            arguments.Add(new ObfuscationFormatOption(ObfuscationFormatType.RandomNumber, argumentsParams));

            var fieldToBeObfuscated = new FieldToBeObfuscated()
            {
                FieldName = "address1_latitude",
                ObfuscationFormat = "{0}"
            };
            fieldToBeObfuscated.ObfuscationFormatArgs.AddRange(arguments);

            return fieldToBeObfuscated;
        }
    }
}