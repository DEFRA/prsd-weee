namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Core.Exceptions;
    using Core.Helpers;
    using Interfaces;
    using Requests.Scheme.MemberRegistration;
    using Xml.Schemas;

    public class XmlConverter : IXmlConverter
    {
        private readonly IWhiteSpaceCollapser whiteSpaceCollapser;
        private readonly IDeserializer deserializer;

        public XmlConverter(IWhiteSpaceCollapser whiteSpaceCollapser, IDeserializer deserializer)
        {
            this.whiteSpaceCollapser = whiteSpaceCollapser;
            this.deserializer = deserializer;
        }

        public XDocument Convert(ProcessXMLFile message)
        {
            return XDocument.Parse(XmlToUtf8String(message), LoadOptions.SetLineInfo);          
        }

        public string XmlToUtf8String(ProcessXMLFile message)
        {
            var data = message.Data;
            data = RemoveUtf8Bom(data);

            return Encoding.UTF8.GetString(data);
        }

        public schemeType Deserialize(XDocument xdoc)
        {
            var scheme = deserializer.Deserialize<schemeType>(xdoc);
            whiteSpaceCollapser.Collapse(scheme);
            scheme.MakeEmptyStringsNull();

            return scheme;
        }

        private byte[] RemoveUtf8Bom(byte[] data)
        {
            var utf8Bom = Encoding.UTF8.GetPreamble();

            if (data.Length < utf8Bom.Length)
            {
                return data;
            }

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
