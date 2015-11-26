namespace EA.Weee.Xml.Tests.Unit.Converter
{
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Deserialization;
    using FakeItEasy;
    using Xml.Converter;
    using Xunit;
    using schemeType = MemberRegistration.schemeType;

    public class XmlConverterTests
    {
        private readonly IDeserializer deserializer;
        private readonly IWhiteSpaceCollapser whiteSpaceCollapser;

        public XmlConverterTests()
        {
            deserializer = A.Fake<IDeserializer>();
            whiteSpaceCollapser = A.Fake<IWhiteSpaceCollapser>();
        }

        [Fact]
        public void XmlWithoutBom_ReturnsXmlContent()
        {
            const string xml = "<root></root>";

            var data = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-16\"?>" + xml);
            var result = XmlConverter().Convert(data);

            Assert.Equal(xml, result.ToString());
        }

        [Fact]
        public void XmlWithBom_ReturnsXmlWithoutBom()
        {
            const string xml = "<root></root>";

            var data = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-16\"?>" + xml)).ToArray();
            var result = XmlConverter().Convert(data);

            Assert.Equal(xml, result.ToString());  
        }

        [Fact]
        public void DeserliazeXml_RemovesWhiteSpaceBeforeChangingEmptyStringsToNull()
        {
            var tradingName = string.Empty;

            var deserializedXml = new schemeType()
            {
                tradingName = tradingName
            };

            A.CallTo(() => deserializer.Deserialize<schemeType>(A<XDocument>._))
                .Returns(deserializedXml);

            A.CallTo(() => whiteSpaceCollapser.Collapse(A<schemeType>._))
                .Invokes(st => tradingName = ((schemeType)st.Arguments[0]).tradingName);

            var result = XmlConverter().Deserialize(A<XDocument>._);

            Assert.NotEqual(tradingName, result.tradingName);
        }

        private XmlConverter XmlConverter()
        {
            return new XmlConverter(whiteSpaceCollapser, deserializer);
        }
    }
}
