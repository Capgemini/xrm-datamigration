using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.DataScrambler.Scramblers.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DecimalScramblerTests
    {
        private DecimalScrambler systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            systemUnderTest = new DecimalScrambler();
        }

        [TestMethod]
        public void Constructor()
        {
            FluentActions.Invoking(() => systemUnderTest = new DecimalScrambler())
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ScramblerEntireDefaultInput()
        {
            decimal input = default;
            var min = 0;
            var max = input.ToString(CultureInfo.InvariantCulture).Length;

            var actual = systemUnderTest.Scramble(input, min, max);

            actual.Should().NotBe(default);
        }

        [TestMethod]
        public void ScramblerEntireInput()
        {
            var input = 452.5M;
            var min = 0;
            var max = input.ToString(CultureInfo.InvariantCulture).Length;

            var actual = systemUnderTest.Scramble(input, min, max);

            actual.Should().NotBe(input);
        }

        [TestMethod]
        public void ScramblerSubsetOfInput()
        {
            var input = 78549.0M;
            var min = 3;
            var max = input.ToString(CultureInfo.InvariantCulture).Length;

            var actual = systemUnderTest.Scramble(input, min, max);

            actual.Should().NotBe(input);
        }

        [TestMethod]
        public void ScramblerMinEqualToMax()
        {
            var input = 7840.9M;
            var min = input.ToString(CultureInfo.InvariantCulture).Length;
            var max = min;

            var actual = systemUnderTest.Scramble(input, min, max);

            actual.Should().NotBe(input);
        }
    }
}