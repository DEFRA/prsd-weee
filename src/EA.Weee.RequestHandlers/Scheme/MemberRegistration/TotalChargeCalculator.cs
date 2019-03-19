namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using Domain.Scheme;
    using EA.Weee.Core.Shared;
    using EA.Weee.Xml.Converter;
    using Interfaces;
    using Requests.Scheme.MemberRegistration;
    using System.Collections.Generic;
    using System.Linq;

    public class TotalChargeCalculator : ITotalChargeCalculator
    {
        private readonly IXMLChargeBandCalculator xmlChargeBandCalculator;
        private readonly IProducerChargeCalculator producerChargerCalculator;
        private readonly IXmlConverter xmlConverter;
        private const int EaComplianceYearCheck = 2018;

        public TotalChargeCalculator(IXMLChargeBandCalculator xmlChargeBandCalculator, IProducerChargeCalculator producerChargerCalculator, IXmlConverter xmlConverter)
        {
            this.xmlChargeBandCalculator = xmlChargeBandCalculator;
            this.producerChargerCalculator = producerChargerCalculator;
            this.xmlConverter = xmlConverter;
        }

        public Dictionary<string, ProducerCharge> TotalCalculatedCharges(ProcessXmlFile message, Scheme scheme, int deserializedcomplianceYear, bool annualChargeToBeAdded, ref decimal? totalCharges)
        {
            var producerCharges = xmlChargeBandCalculator.Calculate(message);

            totalCharges = producerCharges.Aggregate(totalCharges, (current, producerCharge) => current + producerCharge.Value.Amount);

            if (annualChargeToBeAdded && deserializedcomplianceYear > 2018 && scheme.CompetentAuthority.Abbreviation == UKCompetentAuthorityAbbreviationType.EA)
            {
                totalCharges = totalCharges + scheme.CompetentAuthority.AnnualChargeAmount;
            }

            return producerCharges;
        }
    }
}
