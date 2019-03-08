//namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
//{
//    using Domain.Scheme;
//    using EA.Weee.Core.Shared;
//    using Interfaces;
//    using Xml.Converter;
//    public class XmlChargeBandCalculatorStrategy : IXMLChargeBandCalculatorStrategy
//    {
//        private readonly IXmlConverter xmlConverter;
//        private readonly IProducerChargeCalculator producerChargeCalculator;
//        private const int EaComplianceYearCheck = 2018;

//        public XmlChargeBandCalculatorStrategy(IXmlConverter xmlConverter, IProducerChargeCalculator producerChargeCalculator)
//        {
//            this.xmlConverter = xmlConverter;
//            this.producerChargeCalculator = producerChargeCalculator;
//        }

//        public IXMLChargeBandCalculator GetCalculatorOption(Scheme scheme, int complianceYear)
//        {
//            return scheme.CompetentAuthority.Abbreviation == UKCompetentAuthorityAbbreviationType.EA && complianceYear > EaComplianceYearCheck
//                ? new XmlChargeBandCalculatorPost2018(xmlConverter, producerChargeCalculator)
//                : (IXMLChargeBandCalculator)new XmlChargeBandCalculator(xmlConverter, producerChargeCalculator);
//        }
//    }
//}
