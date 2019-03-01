namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using Domain.Scheme;
    using EA.Weee.Core.Shared;
    using EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile;
    using Interfaces;
    using Requests.Scheme.MemberRegistration;
    using System.Collections.Generic;
    using System.Linq;

    public class TotalChargeCalculator : ITotalChargeCalculator
    {
        private readonly IXMLChargeBandCalculator xmlChargeBandCalculator;

        public TotalChargeCalculator(IXMLChargeBandCalculator xmlChargeBandCalculator)
        {
            this.xmlChargeBandCalculator = xmlChargeBandCalculator;
        }

        public Dictionary<string, ProducerCharge> TotalCalculatedCharges(ProcessXmlFile message, Scheme scheme, int complianceYear, bool hasExistingAnnualCharge, int? existingComplianceYear,
                ref bool hasAnnualCharge, ref decimal? totalCharges)
        {
            var producerCharges = xmlChargeBandCalculator.Calculate(message);

            totalCharges = producerCharges
                .Aggregate(totalCharges, (current, producerCharge) => current + producerCharge.Value.Amount);

            if (scheme.CompetentAuthority.Abbreviation == UKCompetentAuthorityAbbreviationType.EA && complianceYear == 2019 && (existingComplianceYear != 2019 && !hasExistingAnnualCharge))
            {
                hasAnnualCharge = true;
                totalCharges = totalCharges + scheme.CompetentAuthority.AnnualChargeAmount;
            }

            return producerCharges;
        }
    }
}
