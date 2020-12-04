using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.DataScrambler.Scramblers.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class GuidScramblerTests
    {
        private GuidScrambler systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            systemUnderTest = new GuidScrambler();
        }

        [TestMethod]
        public void Constructor()
        {
            FluentActions.Invoking(() => systemUnderTest = new GuidScrambler())
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ScramblerEntireEmptyGuidInput()
        {
            var input = Guid.Empty;
            var min = 0;
            var max = input.ToString().Length;

            var actual = systemUnderTest.Scramble(input, min, max);

            actual.Should().NotBe(input);
        }

        [TestMethod]
        public void ScramblerEntireInput()
        {
            var input = Guid.NewGuid();
            var min = 0;
            var max = input.ToString().Length;

            var actual = systemUnderTest.Scramble(input, min, max);

            actual.Should().NotBe(input);
        }

        [TestMethod]
        public void ScramblerSubsetOfInput()
        {
            var input = Guid.NewGuid();
            var min = 3;
            var max = input.ToString().Length;

            var actual = systemUnderTest.Scramble(input, min, max);

            actual.Should().NotBe(input);
        }

        [TestMethod]
        public void ScramblerMinEqualToMaxEqualToZero()
        {
            var input = Guid.NewGuid();
            var min = 0;

            var actual = systemUnderTest.Scramble(input, min, min);

            actual.Should().NotBe(input);
        }

        [TestMethod]
        public void ScramblerMinEqualToMax()
        {
            var input = Guid.NewGuid();
            var min = input.ToString().Length;

            var actual = systemUnderTest.Scramble(input, min, min);

            actual.Should().NotBe(input);
        }
    }
}