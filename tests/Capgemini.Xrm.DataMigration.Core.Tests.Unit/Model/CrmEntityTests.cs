using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Capgemini.Xrm.DataMigration.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.Xrm.DataMigration.Core.Tests.Unit.Model
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CrmEntityTests
    {
        [TestMethod]
        public void CrmEntity()
        {
            FluentActions.Invoking(() => new CrmEntity())
                .Should()
                .NotThrow();
        }

        [TestMethod]
        public void DeserializeCrmEntity()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CrmEntity));
            CrmEntity actual;

            using (FileStream fs = new FileStream("TestData/CrmEntity.xml", FileMode.Open))
            {
                using (XmlReader reader = XmlReader.Create(fs))
                {
                    actual = (CrmEntity)serializer.Deserialize(reader);
                }
            }

            actual.Name.Should().Be("account");
            actual.DisplayName.Should().Be("Account");
            actual.EntityCode.Should().Be("1234");
            actual.PrimaryIdField.Should().Be("accountId");
            actual.PrimaryNameField.Should().Be("name");
            actual.DisablePlugins.Should().Be(true);
            actual.CrmFields.Count.Should().Be(2);
            actual.CrmRelationships.Count.Should().Be(2);
        }
    }
}