namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using Domain.Scheme;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile;
    using EA.Weee.RequestHandlers.Security;
    using Interfaces;
    using Requests.Scheme.MemberRegistration;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TotalChargeCalculator : ITotalChargeCalculator
    {
        private readonly IXMLChargeBandCalculator xmlChargeBandCalculator;
        private ITotalChargeCalculatorDataAccess totalChargeCalculatorDataAccess;
        //private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private const int EAComplianceYearCheck = 2018;
        public TotalChargeCalculator(IXMLChargeBandCalculator xmlChargeBandCalculator, IWeeeAuthorization authorization, ITotalChargeCalculatorDataAccess totalChargeCalculatorDataAccess)
        {
            this.xmlChargeBandCalculator = xmlChargeBandCalculator;
            this.authorization = authorization;
            this.totalChargeCalculatorDataAccess = totalChargeCalculatorDataAccess;
        }

        public Dictionary<string, ProducerCharge> TotalCalculatedCharges(ProcessXmlFile message, Scheme scheme, int deserializedcomplianceYear, ref bool hasAnnualCharge, ref decimal? totalCharges)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            decimal annualcharge = scheme.CompetentAuthority.AnnualChargeAmount ?? 0;

            //var memberUploadsCheckAgainstNotSubmitted = new List<MemberUpload>(totalChargeCalculatorDataAccess.GetMemberUploads(message, false, false, deserializedcomplianceYear));

            //bool checkNotSubmittedNotHasAnnualCharge = totalChargeCalculatorDataAccess.CheckForNotSubmitted(message, scheme, deserializedcomplianceYear, memberUploadsCheckAgainstNotSubmitted);

            //var memberUploadsCheckAgainstSubmitted = new List<MemberUpload>(totalChargeCalculatorDataAccess.GetMemberUploads(message, true, false, deserializedcomplianceYear));

            hasAnnualCharge = totalChargeCalculatorDataAccess.CheckSchemeHasAnnualCharge(scheme, deserializedcomplianceYear);

            var producerCharges = xmlChargeBandCalculator.Calculate(message);

            totalCharges = producerCharges
                .Aggregate(totalCharges, (current, producerCharge) => current + producerCharge.Value.Amount);

            if (!hasAnnualCharge)
            {
                totalCharges = totalCharges + scheme.CompetentAuthority.AnnualChargeAmount;
            }

            return producerCharges;
        }
    }
}
