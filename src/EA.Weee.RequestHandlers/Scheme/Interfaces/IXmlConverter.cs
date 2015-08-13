namespace EA.Weee.RequestHandlers.Scheme.Interfaces
{
    using System.Xml.Linq;
    using Requests.Scheme.MemberRegistration;

    public interface IXmlConverter
    {
        XDocument Convert(ProcessXMLFile message);

        string XmlToUtf8String(ProcessXMLFile message);

        schemeType Deserialize(XDocument xdoc);
    }
}
