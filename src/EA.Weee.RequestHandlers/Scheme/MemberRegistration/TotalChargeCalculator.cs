namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System.Collections.Generic;
    using System.Linq;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile;
    using Interfaces;
    using Requests.Scheme.MemberRegistration;
    public class TotalChargeCalculator : ITotalChargeCalculator
    {
        private readonly IXMLChargeBandCalculator xmlChargeBandCalculator;

        public TotalChargeCalculator()
        {
        }

        public TotalChargeCalculator(IXMLChargeBandCalculator xmlChargeBandCalculator)
        {
            this.xmlChargeBandCalculator = xmlChargeBandCalculator;
        }

        public Dictionary<string, ProducerCharge> TotalCalculatedCharges(ProcessXmlFile message, Scheme scheme, int complianceYear, ref bool hasAnnualCharge, ref decimal? totalCharges)
        {
            var producerCharges = xmlChargeBandCalculator.Calculate(message);

            totalCharges = producerCharges
                .Aggregate(totalCharges, (current, producerCharge) => current + producerCharge.Value.Amount);

            if (scheme.CompetentAuthority.Abbreviation == "EA" && complianceYear == 2019)
            {
                hasAnnualCharge = true;
                totalCharges = totalCharges + scheme.CompetentAuthority.AnnualChargeAmount;
            }

            return producerCharges;
        }
    }
}
