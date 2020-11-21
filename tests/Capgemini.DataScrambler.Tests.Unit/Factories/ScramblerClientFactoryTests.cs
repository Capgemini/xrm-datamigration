using System;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataScrambler.Scramblers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.DataScrambler.Factories.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ScramblerClientFactoryTests
    {
        [TestMethod]
        public void GetScramblerDecimalScrambler()
        {
            var actual = ScramblerClientFactory.GetScrambler<decimal>();

            actual.Should().BeOfType<ScramblerClient<decimal>>();
        }

        [TestMethod]
        public void GetScramblerIntegerScrambler()
        {
            var actual = ScramblerClientFactory.GetScrambler<int>();

            actual.Should().BeOfType<ScramblerClient<int>>();
        }

        [TestMethod]
        public void GetScramblerStringScrambler()
        {
            var actual = ScramblerClientFactory.GetScrambler<string>();

            actual.Should().BeOfType<ScramblerClient<string>>();
        }

        [TestMethod]
        public void GetScramblerDoubleScrambler()
        {
            var actual = ScramblerClientFactory.GetScrambler<double>();

            actual.Should().BeOfType<ScramblerClient<double>>();
        }

        [TestMethod]
        public void GetScramblerEmailScambler()
        {
            FluentActions.Invoking(() => ScramblerClientFactory.GetScrambler<EmailScambler>())
                         .Should()
                         .Throw<NotSupportedException>()
                         .WithMessage($"The specified generic type {nameof(EmailScambler)} could not be found");
        }

        [TestMethod]
        public void GetScramblerString()
        {
            FluentActions.Invoking(() => ScramblerClientFactory.GetScrambler<object>())
                         .Should()
                         .Throw<NotSupportedException>()
                         .WithMessage("The specified generic type object could not be found");
        }
    }
}