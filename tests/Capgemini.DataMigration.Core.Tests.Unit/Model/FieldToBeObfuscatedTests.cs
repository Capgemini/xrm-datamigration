using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core.Model;
using Capgemini.DataMigration.Core.Tests.Base;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.DataMigration.Core.Tests.Unit.Model
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class FieldToBeObfuscatedTests : UnitTestBase
    {
        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
        }

        [TestMethod]
        public void CanBeFormattedReturnsFalseIfFormattingArgsAreMissing()
        {
            FieldToBeObfuscated testObject = new FieldToBeObfuscated() { FieldName = "test" };

            testObject.CanBeFormatted.Should().BeFalse();
        }

        [TestMethod]
        public void CanBeFormattedReturnsTrueIfFormattingArgsAreAvailable()
        {
            Dictionary<string, string> args = new Dictionary<string, string>
            {
                { "length", "10" }
            };
            List<ObfuscationFormatOption> formatOptions = new List<ObfuscationFormatOption>
            {
                new ObfuscationFormatOption(ObfuscationFormatType.RandomString, args)
            };
            var testObject = new FieldToBeObfuscated()
            {
                FieldName = "test",
                ObfuscationFormat = "{0}"
            };
            testObject.ObfuscationFormatArgs.AddRange(formatOptions);

            testObject.CanBeFormatted.Should().BeTrue();
        }
    }
}