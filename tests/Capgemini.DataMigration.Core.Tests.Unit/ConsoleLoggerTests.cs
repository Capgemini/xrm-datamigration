using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.DataMigration.Core.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ConsoleLoggerTests
    {
        private ConsoleLogger systemUnderTest;
        private string message;

        [TestInitialize]
        public void Setup()
        {
            message = "Sample Message";
            systemUnderTest = new ConsoleLogger();
        }

        [TestMethod]
        public void LogError()
        {
            FluentActions.Invoking(() => systemUnderTest.LogError(message))
                        .Should()
                        .NotThrow();
        }

        [TestMethod]
        public void LogErrorWithException()
        {
            var exception = new Exception();

            FluentActions.Invoking(() => systemUnderTest.LogError(message, exception))
                        .Should()
                        .NotThrow();
        }

        [TestMethod]
        public void LogInfo()
        {
            FluentActions.Invoking(() => systemUnderTest.LogInfo(message))
                        .Should()
                        .NotThrow();
        }

        [TestMethod]
        public void LogVerboseTest()
        {
            FluentActions.Invoking(() => systemUnderTest.LogVerbose(message))
                        .Should()
                        .NotThrow();
        }

        [TestMethod]
        public void LogWarningTest()
        {
            FluentActions.Invoking(() => systemUnderTest.LogWarning(message))
                        .Should()
                        .NotThrow();
        }
    }
}