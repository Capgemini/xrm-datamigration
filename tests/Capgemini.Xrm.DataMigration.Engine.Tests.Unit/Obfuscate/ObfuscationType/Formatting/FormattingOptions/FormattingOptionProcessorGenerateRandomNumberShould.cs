using Capgemini.DataMigration.Core.Extensions;
using Capgemini.DataMigration.Core.Model;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting.FormattingOptions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var originalValue = 999999;

            var newValue = systemUnderTest.GenerateRandomNumber(originalValue.ToString(), GenerateValidArgs());

            newValue.Should().NotBe(originalValue);
        }

        [TestMethod]
        public void ThrowArgumentNullExceptionIfArgsParamIsNull()
        {
            var originalValue = 999999;

            Action action = () => systemUnderTest.GenerateRandomString(originalValue.ToString(), null);

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

            var newValue = systemUnderTest.GenerateRandomNumber(originalValue.ToString(), GenerateValidArgs());

            newValue.Should().NotBe(originalValue);
        }

        [TestMethod]
        public void ThrowArgumentOutOfRangeExceptionIfTheMinValueIsHigherThanTheMax()
        {
            var originalValue = 999999;

            Action action = () => systemUnderTest.GenerateRandomNumber(originalValue.ToString(), GenerateInvalidArgs());

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        private static ObfuscationFormatOption GenerateValidArgs()
        {
            var args = new Dictionary<string, string>();
            args.Add("min", MinRangeOfNewNumber.ToString());
            args.Add("max", MaxRangeOfNewNumber.ToString());
            var arg = new ObfuscationFormatOption(ObfuscationFormatType.RandomNumber, args);
            return arg;
        }

        private static ObfuscationFormatOption GenerateInvalidArgs()
        {
            var args = new Dictionary<string, string>();
            args.Add("min", MaxRangeOfNewNumber.ToString());
            args.Add("max", MinRangeOfNewNumber.ToString());
            var arg = new ObfuscationFormatOption(ObfuscationFormatType.RandomString, args);
            return arg;
        }
    }
}
