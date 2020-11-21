using System;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Exceptions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.DataScrambler.Scramblers.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class EmailScamblerTests
    {
        private EmailScambler systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            systemUnderTest = new EmailScambler();
        }

        [TestMethod]
        public void Constructor()
        {
            FluentActions.Invoking(() => systemUnderTest = new EmailScambler())
                         .Should()
                         .NotThrow();
        }

        [TestMethod]
        public void ScramblerNullEmail()
        {
            FluentActions.Invoking(() => systemUnderTest.Scramble(null, 0, 10))
                          .Should()
                          .Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ScramblerInvalidEmail()
        {
            var input = "Test string 1";
            var min = 0;
            var max = input.Length;

            FluentActions.Invoking(() => systemUnderTest.Scramble(input, min, max))
                          .Should()
                          .Throw<ValidationException>()
                          .WithMessage("EmailScambler: This is not a valid email");
        }

        [TestMethod]
        public void ScramblerEntireEmail()
        {
            var input = "Test@email.com";
            var min = 0;
            var max = input.Length;

            var actual = systemUnderTest.Scramble(input, min, max);

            actual.Should().NotBe(input);
        }

        [TestMethod]
        public void ScramblerMinEqualToMaxEqualToZero()
        {
            var input = "Test@email.com";
            var min = 0;

            var actual = systemUnderTest.Scramble(input, min, min);

            actual.Should().NotContain(input);
        }

        [TestMethod]
        public void ScramblerMinEqualToMax()
        {
            var input = "Test@email.com";
            var min = input.Length;

            var actual = systemUnderTest.Scramble(input, min, min);

            actual.Should().NotContain(input);
        }

        [TestMethod]
        public void ScrambleSubsetOfEmail()
        {
            string input = "Test@email.com";
            int min = 5;
            int max = 9;

            var actual = systemUnderTest.Scramble(input, min, max);

            actual.Should().NotBe(input);
        }
    }
}