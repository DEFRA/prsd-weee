namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using Domain.Scheme;
    using EA.Weee.Core.Shared;
    using EA.Weee.Xml.Converter;
    using Interfaces;
    using Requests.Scheme.MemberRegistration;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xml.MemberRegistration;

    public class TotalChargeCalculator : ITotalChargeCalculator
    {
        private readonly IXMLChargeBandCalculator xmlChargeBandCalculator;
        private readonly IXmlConverter xmlConverter;
        private readonly IAnnualChargeDataAccess annualChargeDataAccess;
        private const int EaComplianceYearCheck = 2018;

        public TotalChargeCalculator(IXMLChargeBandCalculator xmlChargeBandCalculator, IXmlConverter xmlConverter, IAnnualChargeDataAccess annualChargeDataAccess)
        {
            this.xmlChargeBandCalculator = xmlChargeBandCalculator;
            this.xmlConverter = xmlConverter;
            this.annualChargeDataAccess = annualChargeDataAccess;
        }

        public Dictionary<string, ProducerCharge> TotalCalculatedCharges(ProcessXmlFile message, Scheme scheme, int deserializedcomplianceYear, bool annualChargeToBeAdded, ref decimal? totalCharges)
        {
            var producerCharges = xmlChargeBandCalculator.Calculate(message);

            totalCharges = producerCharges.Aggregate(totalCharges, (current, producerCharge) => current + producerCharge.Value.Amount);

            if (annualChargeToBeAdded &&
                deserializedcomplianceYear > EaComplianceYearCheck &&
                scheme.CompetentAuthority.Abbreviation == UKCompetentAuthorityAbbreviationType.EA)
            {
                // Get year-specific annual charge from the database
                var annualCharge = Task.Run(() =>
                    annualChargeDataAccess.GetAnnualChargeForComplianceYear(
                        scheme.CompetentAuthority.Id,
                        deserializedcomplianceYear)).Result;

                if (annualCharge.HasValue)
                {
                    totalCharges = totalCharges + annualCharge.Value;
                }
            }

            return producerCharges;
        }
    }
}
