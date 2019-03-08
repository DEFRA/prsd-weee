namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using Domain.Scheme;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Xml.Converter;
    using Interfaces;
    using Requests.Scheme.MemberRegistration;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TotalChargeCalculator : ITotalChargeCalculator
    {
        private readonly IXMLChargeBandCalculator xmlChargeBandCalculator;
        private readonly ITotalChargeCalculatorDataAccess totalChargeCalculatorDataAccess;
        private readonly IProducerChargeCalculator producerChargerCalculator;
        private readonly IXmlConverter xmlConverter;
        private const int EaComplianceYearCheck = 2018;

        public TotalChargeCalculator(IXMLChargeBandCalculator xmlChargeBandCalculator, ITotalChargeCalculatorDataAccess totalChargeCalculatorDataAccess, 
            IProducerChargeCalculator producerChargerCalculator, IXmlConverter xmlConverter)
        {
            this.xmlChargeBandCalculator = xmlChargeBandCalculator;
            this.totalChargeCalculatorDataAccess = totalChargeCalculatorDataAccess;
            this.producerChargerCalculator = producerChargerCalculator;
            this.xmlConverter = xmlConverter;
        }

        public Dictionary<string, ProducerCharge> TotalCalculatedCharges(ProcessXmlFile message, Scheme scheme, int deserializedcomplianceYear, ref bool hasAnnualCharge, ref decimal? totalCharges)
        {
            var annualcharge = scheme.CompetentAuthority.AnnualChargeAmount ?? 0;

            hasAnnualCharge = totalChargeCalculatorDataAccess.CheckSchemeHasAnnualCharge(scheme, deserializedcomplianceYear);

            var producerCharges = new Dictionary<string, ProducerCharge>();
            var xmlChargeBandCalculatorContext = new XMLChargeBandCalculatorContext();

            var calculatorOptionStrategy = new XmlChargeBandCalculatorStrategy(xmlConverter, producerChargerCalculator);
            var calculatorOption = calculatorOptionStrategy.GetCalculatorOption(scheme, deserializedcomplianceYear);
            producerCharges = calculatorOption.Calculate(message);

            totalCharges = producerCharges.Aggregate(totalCharges, (current, producerCharge) => current + producerCharge.Value.Amount);

            if (!hasAnnualCharge && deserializedcomplianceYear > 2018)
            {
                totalCharges = totalCharges + scheme.CompetentAuthority.AnnualChargeAmount;
                hasAnnualCharge = true;
            }

            return producerCharges;
        }
    }
}
