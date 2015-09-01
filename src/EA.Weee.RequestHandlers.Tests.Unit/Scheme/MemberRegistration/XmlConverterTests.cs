namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using System.Linq;
    using System.Text;
    using Core.Exceptions;
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

        [Fact]
        public void InvalidXml_ThrowsXmlDeserializationFailureException()
        {
            var data = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-16\"?><root></root>")).ToArray();
            var document = XmlConverter().Convert(new ProcessXMLFile(Guid.NewGuid(), data));

            var exception = Assert.Throws<XmlDeserializationFailureException>(() => XmlConverter().Deserialize(document));
            Assert.NotNull(exception.InnerException);
        }

        private XmlConverter XmlConverter()
        {
            return new XmlConverter();
        }
    }
}
