namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using Domain;
    using Domain.Scheme;
    using Interfaces;
    using Requests.Scheme.MemberRegistration;
    using System;
    using System.Collections.Generic;
    using Xml.Converter;
    using Xml.MemberRegistration;

    public class XmlChargeBandCalculator : IXmlChargeBandCalculator
    {
        private readonly IXmlConverter xmlConverter;
        private readonly IProducerChargeCalculator producerChargeCalculator;
        public List<MemberUploadError> ErrorsAndWarnings { get; set; }

        public XmlChargeBandCalculator(IXmlConverter xmlConverter, IProducerChargeCalculator producerChargeCalculator)
        {
            this.xmlConverter = xmlConverter;
            this.producerChargeCalculator = producerChargeCalculator;
            ErrorsAndWarnings = new List<MemberUploadError>();
        }

        public Dictionary<string, ProducerCharge> Calculate(ProcessXMLFile message)
        {
            var schemeType = xmlConverter.Deserialize(xmlConverter.Convert(message.Data));

            var producerCharges = new Dictionary<string, ProducerCharge>();
            var complianceYear = Int32.Parse(schemeType.complianceYear);

            foreach (var producer in schemeType.producerList)
            {
                var producerName = producer.GetProducerName();
                var producerCharge = producerChargeCalculator.CalculateCharge(schemeType.approvalNo, producer, complianceYear);
                if (producerCharge != null)
                {
                    if (!producerCharges.ContainsKey(producerName))
                    {
                        producerCharges.Add(producerName, producerCharge);
                    }
                    else
                    {
                        ErrorsAndWarnings.Add(
                            new MemberUploadError(
                                ErrorLevel.Error,
                                MemberUploadErrorType.Business,
                                string.Format(
                                    "We are unable to check for warnings associated with the charge band of the producer {0} until the duplicate name has been fixed.",
                                    producerName)));
                    }
                }
            }

            return producerCharges;
        }
    }
}
