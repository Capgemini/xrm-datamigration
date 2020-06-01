using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.DataMigration.Exceptions.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ValidationExceptionTests
    {
        private ValidationException systemUnderTest;

        [TestMethod]
        public void ValidationException()
        {
            FluentActions.Invoking(() => systemUnderTest = new ValidationException())
                 .Should()
                 .NotThrow();
        }

        [TestMethod]
        public void ValidationExceptionWithStringParameter()
        {
            var message = "Test message";

            FluentActions.Invoking(() => systemUnderTest = new ValidationException(message))
                 .Should()
                 .NotThrow();

            Assert.AreEqual(message, systemUnderTest.Message);
        }

        [TestMethod]
        public void ValidationExceptionWithStringAndInnerException()
        {
            var message = "Test message";

            FluentActions.Invoking(() => systemUnderTest = new ValidationException(message, new Exception()))
                 .Should()
                 .NotThrow();

            Assert.AreEqual(message, systemUnderTest.Message);
            Assert.IsNotNull(systemUnderTest.InnerException);
        }
    }
}