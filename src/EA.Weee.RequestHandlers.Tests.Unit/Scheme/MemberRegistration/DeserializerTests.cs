namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Core.Exceptions;
    using RequestHandlers.Scheme.MemberRegistration;
    using Requests.Scheme.MemberRegistration;
    using Xml.Schemas;
    using Xunit;

    public class DeserializerTests
    {
        [Fact]
        public void InvalidXml_ThrowsXmlDeserializationFailureException()
        {
            var deserializer = new Deserializer();

            var xml = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-16\"?><root></root>");

            var exception = Assert.Throws<XmlDeserializationFailureException>(() => deserializer.Deserialize<schemeType>(xml));
            Assert.NotNull(exception.InnerException);
        }
    }
}
