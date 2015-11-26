namespace EA.Weee.RequestHandlers.Scheme.Interfaces
{
    using Requests.Scheme.MemberRegistration;
    using System.Xml.Linq;
    using Xml.MemberRegistration;

    public interface IXmlConverter
    {
        XDocument Convert(ProcessXMLFile message);

        string XmlToUtf8String(ProcessXMLFile message);

        schemeType Deserialize(XDocument xdoc);
    }
}
