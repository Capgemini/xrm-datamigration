using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Capgemini.DataMigration.Core.Extensions;
using Microsoft.Xrm.Sdk.Query;

namespace Capgemini.Xrm.DataMigration.Extensions
{
    public static class FetchExpressionExtensions
    {
        public static FetchExpression ApplyPagingDetails(this FetchExpression fetchXml, string cookie, int page, int count)
        {
            fetchXml.ThrowArgumentNullExceptionIfNull(nameof(fetchXml));

            StringReader stringReader = new StringReader(fetchXml.Query);
            using (XmlTextReader reader = new XmlTextReader(stringReader))
            {
                reader.DtdProcessing = DtdProcessing.Prohibit;

                XmlDocument doc = new XmlDocument
                {
                    XmlResolver = null
                };
                doc.Load(reader);

                fetchXml.Query = CreateXml(doc, cookie, page, count);
            }

            return fetchXml;
        }

        private static string CreateXml(XmlDocument doc, string cookie, int page, int count)
        {
            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(page, CultureInfo.InvariantCulture);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(count, CultureInfo.InvariantCulture);
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
        }
    }
}