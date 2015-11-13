namespace EA.Weee.Xml.Tests.Unit.CustomTypes
{
    using System.IO;
    using System.Xml;
    using Xml.CustomTypes;
    using Xunit;

    public class XmlNullableTests
    {
        [Fact]
        public void ReadXml_NoXmlEntryIsInterpretedAsNull()
        {
            var xml = @"<test/>";

            using (var stringReader = new StringReader(xml))
            {
                var xmlReader = XmlReader.Create(stringReader);

                var xmlNullable = new XmlNullable<decimal>(); // Decimal example
                
                xmlNullable.ReadXml(xmlReader);

                Assert.False(xmlNullable.HasValue);
            }
        }

        [Fact]
        public void ReadXml_EmptyEntryIsInterpretedAsNull()
        {
            var xml = @"<test></test>";

            using (var stringReader = new StringReader(xml))
            {
                var xmlReader = XmlReader.Create(stringReader);

                var xmlNullable = new XmlNullable<decimal>(); // Decimal example

                xmlNullable.ReadXml(xmlReader);

                Assert.False(xmlNullable.HasValue);
            }
        }

        [Fact]
        public void ReadXml_NonEmptyEntryHasValue()
        {
            const decimal dec = 12345678.87654321M;

            var xml = string.Format("<test>{0}</test>", dec);

            using (var stringReader = new StringReader(xml))
            {
                var xmlReader = XmlReader.Create(stringReader);

                var xmlNullable = new XmlNullable<decimal>(); // Decimal example

                xmlNullable.ReadXml(xmlReader);

                Assert.True(xmlNullable.HasValue);
                Assert.Equal(dec, xmlNullable.Value);
            }
        }
    }
}
