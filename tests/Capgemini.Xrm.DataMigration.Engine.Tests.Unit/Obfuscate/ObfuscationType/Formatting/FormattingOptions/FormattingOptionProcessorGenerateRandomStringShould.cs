using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capgemini.Xrm.DataMigration.Engine.Obfuscate.ObfuscationType.Formatting.FormattingOptions;
using FluentAssertions;
using Capgemini.DataMigration.Core.Model;
using System.Threading;
using Capgemini.DataMigration.Core.Tests.Base;

namespace Capgemini.Xrm.DataMigration.Engine.Tests.Unit.Obfuscate.ObfuscationType.Formatting.FormattingOptions
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class FormattingOptionProcessorGenerateRandomStringShould : UnitTestBase
    {
        private const int MaxLengthOfString = 10;
        private FormattingOptionProcessor systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
            systemUnderTest = new FormattingOptionProcessor();
        }

        [TestMethod]
        public void GenerateRandomStringThatIsDifferentToTheOriginalValue()
        {
            var originalValue = "Tester";

            var newValue = systemUnderTest.GenerateRandomString(originalValue, GenerateValidArgs());

            newValue.Should().NotBe(originalValue);
        }

        [TestMethod]
        public void GenerateRandomStringUsingTheDefaultProcessIfRequiredArgsAreMissing()
        {
            var originalValue = "Tester";

            var newValue = systemUnderTest.GenerateRandomString(originalValue, GenerateInvalidArgs());

            newValue.Should().NotBe(originalValue);
        }

        [TestMethod]
        public void GenerateRandomStringThatIsTheLengthSetInTheArgs()
        {
            var originalValue = "StringGreaterThan10Chars";

            var newValue = systemUnderTest.GenerateRandomString(originalValue, GenerateValidArgs());

            newValue.Length.Should().Be(MaxLengthOfString);
        }

        [TestMethod]
        public void GenerateRandomStringThatIsNotRelatedToTheOriginalValue()
        {
            var originalValue = "Tester";

            var firstNewValue = systemUnderTest.GenerateRandomString(originalValue, GenerateValidArgs());

            Thread.Sleep(200);

            var secondNewValue = systemUnderTest.GenerateRandomString(originalValue, GenerateValidArgs());

            originalValue.Should().NotBe(firstNewValue);
            originalValue.Should().NotBe(secondNewValue);
            secondNewValue.Should().NotBe(firstNewValue);
        }

        [TestMethod]
        public void ThrowArgumentNullExceptionIfArgsParamIsNull()
        {
            var originalValue = "Tester";

            Action action = () => systemUnderTest.GenerateRandomString(originalValue, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ThrowArgumentNullExceptionIfOriginalValueIsNull()
        {
            Action action = () => systemUnderTest.GenerateRandomString(null, GenerateValidArgs());

            action.Should().Throw<ArgumentNullException>();
        }

        private static ObfuscationFormatOption GenerateValidArgs()
        {
            var args = new Dictionary<string, string>();
            args.Add("length", MaxLengthOfString.ToString());
            var arg = new ObfuscationFormatOption(ObfuscationFormatType.RandomString, args);
            return arg;
        }

        private static ObfuscationFormatOption GenerateInvalidArgs()
        {
            var args = new Dictionary<string, string>();
            args.Add("width", MaxLengthOfString.ToString());
            var arg = new ObfuscationFormatOption(ObfuscationFormatType.RandomString, args);
            return arg;
        }
    }
}
