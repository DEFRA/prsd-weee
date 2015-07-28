namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Requests.PCS.MemberRegistration;

    public class XmlChargeBandCalculator : IXmlChargeBandCalculator
    {
        public decimal Calculate(ProcessXMLFile message)
        {
            var doc = XDocument.Parse(message.Data, LoadOptions.SetLineInfo);
            schemeType schemeType = (schemeType)new XmlSerializer(typeof(schemeType)).Deserialize(doc.CreateReader());
            var producerChargeBandCalculator = new ProducerChargeBandCalculator();
            return producerChargeBandCalculator.CalculateCharge(schemeType.producerList);
        }
    }
}
