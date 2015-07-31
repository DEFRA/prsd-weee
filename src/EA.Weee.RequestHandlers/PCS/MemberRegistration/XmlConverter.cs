namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Requests.PCS.MemberRegistration;

    public class XmlConverter : IXmlConverter
    {
        public XDocument Convert(ProcessXMLFile message)
        {
            var data = message.Data;
            data = RemoveUtf8Bom(data);

            var xml = Encoding.UTF8.GetString(data);
            return XDocument.Parse(xml, LoadOptions.SetLineInfo);          
        }

        public schemeType Deserialize(XDocument xdoc)
        {
            return (schemeType)new XmlSerializer(typeof(schemeType)).Deserialize(xdoc.CreateReader());
        }

        private byte[] RemoveUtf8Bom(byte[] data)
        {
            var utf8Bom = Encoding.UTF8.GetPreamble();
            if (utf8Bom.Where((t, i) => data[i] != t).Any())
            {
                return data;
            }

            var filteredData = new byte[data.Length - utf8Bom.Length];
            for (var i = 0; i < data.Length - utf8Bom.Length; i++)
            {
                filteredData[i] = data[i + utf8Bom.Length];
            }

            return filteredData;
        }
    }
}
