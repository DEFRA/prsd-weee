namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Requests.PCS.MemberRegistration;

    public class XmlConverter : IXmlConverter
    {
        public schemeType Convert(ProcessXmlFile message)
        {
            var doc = XDocument.Parse(message.Data, LoadOptions.SetLineInfo);
            return (schemeType)new XmlSerializer(typeof(schemeType)).Deserialize(doc.CreateReader());
        }
    }
}
