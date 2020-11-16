using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Capgemini.DataMigration.Core.Model;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting.FormattingOptions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.Obfuscate.ObfuscationType.Formatting.FormattingOptions
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class FormattingOptionProcessorGenerateRandomNumberShould
    {
        private const int MinRangeOfNewNumber = 1000;
        private const int MaxRangeOfNewNumber = 2000;
        private FormattingOptionProcessor systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            systemUnderTest = new FormattingOptionProcessor();
        }

        [TestMethod]
        public void GenerateARandomNumberThatIsDifferentToTheOriginalValue()
        {
            var originalValue = 867489;

            var newValue = FormattingOptionProcessor.GenerateRandomNumber(originalValue.ToString(CultureInfo.InvariantCulture), GenerateValidArgs());

            newValue.Should().NotBe(originalValue);
        }

        [TestMethod]
        public void ThrowArgumentNullExceptionIfArgsParamIsNull()
        {
            var originalValue = 999999;

            Action action = () => systemUnderTest.GenerateRandomString(originalValue.ToString(CultureInfo.InvariantCulture), null);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ThrowArgumentNullExceptionIfOriginalValueIsNull()
        {
            Action action = () => systemUnderTest.GenerateRandomString(null, GenerateValidArgs());

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ReplaceTheDefaultMinMaxValuesIfThosArgumentsArePassed()
        {
            var originalValue = 999999;

            var newValue = FormattingOptionProcessor.GenerateRandomNumber(originalValue.ToString(CultureInfo.InvariantCulture), GenerateValidArgs());

            newValue.Should().NotBe(originalValue);
        }

        [TestMethod]
        public void ThrowArgumentOutOfRangeExceptionIfTheMinValueIsHigherThanTheMax()
        {
            var originalValue = 999999;

            Action action = () => FormattingOptionProcessor.GenerateRandomNumber(originalValue.ToString(CultureInfo.InvariantCulture), GenerateInvalidArgs());

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        private static ObfuscationFormatOption GenerateValidArgs()
        {
            var args = new Dictionary<string, string>
            {
                { "min", MinRangeOfNewNumber.ToString(CultureInfo.InvariantCulture) },
                { "max", MaxRangeOfNewNumber.ToString(CultureInfo.InvariantCulture) }
            };
            var arg = new ObfuscationFormatOption(ObfuscationFormatType.RandomNumber, args);
            return arg;
        }

        private static ObfuscationFormatOption GenerateInvalidArgs()
        {
            var args = new Dictionary<string, string>
            {
                { "min", MaxRangeOfNewNumber.ToString(CultureInfo.InvariantCulture) },
                { "max", MinRangeOfNewNumber.ToString(CultureInfo.InvariantCulture) }
            };
            var arg = new ObfuscationFormatOption(ObfuscationFormatType.RandomString, args);
            return arg;
        }
    }
}