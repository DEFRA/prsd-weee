namespace EA.Weee.Xml.Converter
{
    using System.Xml.Linq;
    using Xml.Schemas;

    public interface IXmlConverter
    {
        XDocument Convert(byte[] data);

        string XmlToUtf8String(byte[] data);

        schemeType Deserialize(XDocument xdoc);
    }
}
