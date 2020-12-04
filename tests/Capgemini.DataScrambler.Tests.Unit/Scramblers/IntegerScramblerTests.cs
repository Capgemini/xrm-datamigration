using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.DataScrambler.Scramblers.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class IntegerScramblerTests
    {
        private IntegerScrambler systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            systemUnderTest = new IntegerScrambler();
        }

        [TestMethod]
        public void Constructor()
        {
            FluentActions.Invoking(() => systemUnderTest = new IntegerScrambler())
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ScramblerEntireInput()
        {
            var input = 452;
            var min = 0;
            var max = input.ToString(CultureInfo.InvariantCulture).Length;

            var actual = systemUnderTest.Scramble(input, min, max);

            actual.Should().NotBe(input);
        }

        [TestMethod]
        public void ScramblerSubsetOfInput()
        {
            var input = 78549;
            var min = 3;
            var max = input.ToString(CultureInfo.InvariantCulture).Length;

            var actual = systemUnderTest.Scramble(input, min, max);

            actual.Should().NotBe(input);
        }

        [TestMethod]
        public void ScramblerMinEqualToMax()
        {
            var input = 7840;
            var min = input.ToString(CultureInfo.InvariantCulture).Length;
            var max = min;

            var actual = systemUnderTest.Scramble(input, min, max);

            actual.Should().NotBe(input);
        }
    }
}