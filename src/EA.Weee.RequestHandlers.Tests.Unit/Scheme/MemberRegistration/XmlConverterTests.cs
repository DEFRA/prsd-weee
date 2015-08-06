namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using System.Linq;
    using System.Text;
    using RequestHandlers.Scheme.MemberRegistration;
    using Requests.Scheme.MemberRegistration;
    using Xunit;

    public class XmlConverterTests
    {
        [Fact]
        public void XmlWithoutBom_ReturnsXmlContent()
        {
            const string xml = "<root></root>";

            var data = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-16\"?>" + xml);
            var result = XmlConverter().Convert(new ProcessXMLFile(Guid.NewGuid(), data));

            Assert.Equal(xml, result.ToString());
        }

        [Fact]
        public void XmlWithBom_ReturnsXmlWithoutBom()
        {
            const string xml = "<root></root>";

            var data = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-16\"?>" + xml)).ToArray();
            var result = XmlConverter().Convert(new ProcessXMLFile(Guid.NewGuid(), data));

            Assert.Equal(xml, result.ToString());  
        }

        private XmlConverter XmlConverter()
        {
            return new XmlConverter();
        }
    }
}
