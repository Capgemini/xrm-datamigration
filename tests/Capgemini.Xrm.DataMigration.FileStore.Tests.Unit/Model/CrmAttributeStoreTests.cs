using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.FileStore.Model.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmAttributeStoreTests
    {
        private CrmAttributeStore systemUnderTest = null;

        [TestMethod]
        public void CrmAttributeStore()
        {
            FluentActions.Invoking(() => systemUnderTest = new CrmAttributeStore())
                             .Should()
                             .NotThrow();

            Assert.IsNull(systemUnderTest.AttributeName);
            Assert.IsNull(systemUnderTest.AttributeValue);
            Assert.IsNull(systemUnderTest.AttributeType);
        }

        [TestMethod]
        public void CrmAttributeStoreWithParameter()
        {
            var attribute = new KeyValuePair<string, object>("contactid", Guid.NewGuid());

            FluentActions.Invoking(() => systemUnderTest = new CrmAttributeStore(attribute))
                             .Should()
                             .NotThrow();

            Assert.AreEqual(attribute.Key, systemUnderTest.AttributeName);
            Assert.AreEqual(attribute.Value.ToString(), systemUnderTest.AttributeValue.ToString());
        }
    }
}