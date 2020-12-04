using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.DataScrambler.Scramblers.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DoubleScramblerTests
    {
        private DoubleScrambler systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            systemUnderTest = new DoubleScrambler();
        }

        [TestMethod]
        public void Constructor()
        {
            FluentActions.Invoking(() => systemUnderTest = new DoubleScrambler())
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ScramblerEntireDefaultInput()
        {
            double input = default;
            var min = 0;
            var max = input.ToString(CultureInfo.InvariantCulture).Length;

            var actual = systemUnderTest.Scramble(input, min, max);

            actual.Should().NotBe(default);
        }

        [TestMethod]
        public void ScramblerEntireInput()
        {
            var input = 452.689;
            var min = 0;
            var max = input.ToString(CultureInfo.InvariantCulture).Length;

            var actual = systemUnderTest.Scramble(input, min, max);

            actual.Should().NotBe(input);
        }

        [TestMethod]
        public void ScramblerSubsetOfInput()
        {
            var input = 78549.1458;
            var min = 3;
            var max = input.ToString(CultureInfo.InvariantCulture).Length;

            var actual = systemUnderTest.Scramble(input, min, max);

            actual.Should().NotBe(input);
        }

        [TestMethod]
        public void ScramblerMinEqualToMax()
        {
            var input = 7840.897;
            var min = input.ToString(CultureInfo.InvariantCulture).Length;
            var max = min;

            var actual = systemUnderTest.Scramble(input, min, max);

            actual.Should().NotBe(input);
        }
    }
}