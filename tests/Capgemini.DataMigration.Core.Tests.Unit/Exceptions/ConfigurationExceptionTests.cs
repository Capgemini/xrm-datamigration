using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.DataMigration.Exceptions.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ConfigurationExceptionTests
    {
        private ConfigurationException systemUnderTest;

        [TestMethod]
        public void ConfigurationException()
        {
            FluentActions.Invoking(() => systemUnderTest = new ConfigurationException())
                 .Should()
                 .NotThrow();
        }

        [TestMethod]
        public void ConfigurationExceptionWithStringParameter()
        {
            var message = "Test message";

            FluentActions.Invoking(() => systemUnderTest = new ConfigurationException(message))
                 .Should()
                 .NotThrow();

            Assert.AreEqual(message, systemUnderTest.Message);
        }

        [TestMethod]
        public void ConfigurationExceptionWithStringAndInnerException()
        {
            var message = "Test message";

            FluentActions.Invoking(() => systemUnderTest = new ConfigurationException(message, new Exception()))
                 .Should()
                 .NotThrow();

            Assert.AreEqual(message, systemUnderTest.Message);
            Assert.IsNotNull(systemUnderTest.InnerException);
        }
    }
}