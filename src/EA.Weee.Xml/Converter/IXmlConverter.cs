namespace EA.Weee.Xml.Converter
{
    using System.Xml.Linq;
    using MemberRegistration;

    public interface IXmlConverter
    {
        XDocument Convert(byte[] data);

        string XmlToUtf8String(byte[] data);

        schemeType Deserialize(XDocument xdoc);
    }
}
