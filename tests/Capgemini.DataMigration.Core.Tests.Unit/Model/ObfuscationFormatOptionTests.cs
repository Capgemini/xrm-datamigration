using Capgemini.DataMigration.Core.Model;
using Capgemini.DataMigration.Core.Tests.Base;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capgemini.DataMigration.Core.Tests.Unit.Model
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ObfuscationFormatOptionTests : UnitTestBase
    {
        [TestInitialize]
        public void Setup()
        {
            InitializeProperties();
        }

        [TestMethod]
        public void GetArgumentsShouldReturnTheArgumentsPassedInWhenInitialisingTheObject()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("length", "10");
            ObfuscationFormatOption testObject = new ObfuscationFormatOption(ObfuscationFormatType.RandomString, args);

            testObject.Arguments.Should().BeEquivalentTo(args);
        }

        [TestMethod]
        public void GetFormatTypeShouldReturnTheFormatTypePassedInWhenInitialisingTheObject()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("length", "10");
            ObfuscationFormatOption testObject = new ObfuscationFormatOption(ObfuscationFormatType.RandomString, args);

            testObject.FormatType.Should()
                .BeOfType(typeof(ObfuscationFormatType));

            testObject.FormatType
                .Should().BeEquivalentTo(ObfuscationFormatType.RandomString);
        }
    }
}
