using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.DataScrambler.Scramblers.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class StringScramblerTests
    {
        private StringScrambler systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            systemUnderTest = new StringScrambler();
        }

        [TestMethod]
        public void Constructor()
        {
            FluentActions.Invoking(() => systemUnderTest = new EmailScambler())
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ScramblerNullString()
        {
            FluentActions.Invoking(() => systemUnderTest.Scramble(null, 0, 10))
                          .Should()
                          .Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ScramblerEntireString()
        {
            var input = "Test string 1";
            var min = 0;
            var max = input.Length;

            var actual = systemUnderTest.Scramble(input, min, max);

            actual.Should().NotBe(input);
        }

        [TestMethod]
        public void ScramblerSubsetOfString()
        {
            var input = "Test string 1";
            var min = 3;
            var max = input.Length;

            var actual = systemUnderTest.Scramble(input, min, max);

            actual.Should().NotContain(input);
        }

        [TestMethod]
        public void ScramblerMinEqualToMaxEqualToZero()
        {
            var input = "Test string 1";
            var min = 0;

            var actual = systemUnderTest.Scramble(input, min, min);

            actual.Should().NotContain(input);
        }

        [TestMethod]
        public void ScramblerMinEqualToMax()
        {
            var input = "Test string 1";
            var min = input.Length;

            var actual = systemUnderTest.Scramble(input, min, min);

            actual.Should().NotContain(input);
        }
    }
}