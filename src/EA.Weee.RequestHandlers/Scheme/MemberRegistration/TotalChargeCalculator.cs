namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System.Collections.Generic;
    using System.Linq;
    using Domain.Scheme;
    using Interfaces;
    using Requests.Scheme.MemberRegistration;
    internal class TotalChargeCalculator
    {
        private readonly IXMLChargeBandCalculator xmlChargeBandCalculator;
        
        public TotalChargeCalculator(IXMLChargeBandCalculator xmlChargeBandCalculator)
        {
            this.xmlChargeBandCalculator = xmlChargeBandCalculator;
        }

        internal Dictionary<string, ProducerCharge> TotalCalculatedCharges(ProcessXmlFile message, Scheme scheme, int complianceYear, ref bool hasAnnualCharge, ref decimal totalCharges)
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
