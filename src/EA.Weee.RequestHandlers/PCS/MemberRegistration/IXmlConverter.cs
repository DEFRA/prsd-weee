namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System.Xml.Linq;
    using Requests.PCS.MemberRegistration;

    public interface IXmlConverter
    {
        XDocument Convert(ProcessXMLFile message);

        string XmlToUtf8String(ProcessXMLFile message);

        schemeType Deserialize(XDocument xdoc);
    }
}
