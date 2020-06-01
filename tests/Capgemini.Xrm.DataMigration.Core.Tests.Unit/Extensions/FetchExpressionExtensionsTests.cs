using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Query;

namespace Capgemini.Xrm.DataMigration.Extensions.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class FetchExpressionExtensionsTests
    {
        [TestMethod]
        public void ApplyPagingDetailsNullFetchExpression()
        {
            string cookie = string.Empty;
            int page = 1;
            int count = 10;

            FetchExpression fetchExpression = null;

            FluentActions.Invoking(() => fetchExpression.ApplyPagingDetails(cookie, page, count))
                        .Should()
                        .Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ApplyPagingDetailsNullFetchExpressionQuery()
        {
            string cookie = string.Empty;
            int page = 1;
            int count = 10;

            var fetchExpression = new FetchExpression();

            FluentActions.Invoking(() => fetchExpression.ApplyPagingDetails(cookie, page, count))
                        .Should()
                        .Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ApplyPagingDetailsEmptyFetchExpressionQuery()
        {
            string cookie = string.Empty;
            int page = 1;
            int count = 10;

            var fetchExpression = new FetchExpression
            {
                Query = string.Empty
            };

            FluentActions.Invoking(() => fetchExpression.ApplyPagingDetails(cookie, page, count))
                        .Should()
                        .Throw<XmlException>();
        }

        [TestMethod]
        public void ApplyPagingDetails()
        {
            string cookie = "testcookie";
            int page = 1;
            int count = 10;

            var fetchExpression = new FetchExpression
            {
                Query = "<fetch><entity name=\"contact\"><attribute name=\"firstname\" /><attribute name=\"lastname\" /></entity></fetch>"
            };

            FetchExpression actual = null;

            FluentActions.Invoking(() => actual = fetchExpression.ApplyPagingDetails(cookie, page, count))
                        .Should()
                        .NotThrow();

            actual.Query.Should().Contain($"page=\"{page}\"");
            actual.Query.Should().Contain($"count=\"{count}\"");
            actual.Query.Should().Contain($"paging-cookie=\"{cookie}\"");
        }

        [TestMethod]
        public void ApplyPagingDetailsWithNoCookie()
        {
            string cookie = null;
            int page = 1;
            int count = 10;

            var fetchExpression = new FetchExpression
            {
                Query = "<fetch><entity name=\"contact\"><attribute name=\"firstname\" /><attribute name=\"lastname\" /></entity></fetch>"
            };

            FetchExpression actual = null;

            FluentActions.Invoking(() => actual = fetchExpression.ApplyPagingDetails(cookie, page, count))
                        .Should()
                        .NotThrow();

            actual.Query.Should().Contain($"page=\"{page}\"");
            actual.Query.Should().Contain($"count=\"{count}\"");
            actual.Query.Should().NotContain($"paging-cookie=\"");
        }
    }
}