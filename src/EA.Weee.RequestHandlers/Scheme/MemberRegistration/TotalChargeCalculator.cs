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
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private const int EAComplianceYearCheck = 2018;
        public TotalChargeCalculator(IXMLChargeBandCalculator xmlChargeBandCalculator, WeeeContext context, IWeeeAuthorization authorization)
        {
            this.xmlChargeBandCalculator = xmlChargeBandCalculator;
            this.context = context;
            this.authorization = authorization;
        }

        public Dictionary<string, ProducerCharge> TotalCalculatedCharges(ProcessXmlFile message, Scheme scheme, int deserializedcomplianceYear, ref bool hasAnnualCharge, ref decimal? totalCharges)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            decimal annualcharge = scheme.CompetentAuthority.AnnualChargeAmount ?? 0;

            CheckPreviousAnnualChargeAndReset(message, annualcharge, deserializedcomplianceYear);

            var memberUploadsCheckAgainstNotSubmitted = new List<MemberUpload>(GetMemberUploads(message, false, false, deserializedcomplianceYear));

            var checkNotSubmittedNotHasAnnualCharge = scheme.CompetentAuthority.Abbreviation == UKCompetentAuthorityAbbreviationType.EA &&
                deserializedcomplianceYear > EAComplianceYearCheck && scheme.OrganisationId == message.OrganisationId &&
                memberUploadsCheckAgainstNotSubmitted.Any(m => !m.HasAnnualCharge) &&
                memberUploadsCheckAgainstNotSubmitted.Any(m => !m.IsSubmitted); 
                
            var memberUploadsCheckAgainstSubmitted = new List<MemberUpload>(GetMemberUploads(message, true, true, deserializedcomplianceYear));

            var checkIsSubmittedAndHasAnnualCharge = scheme.CompetentAuthority.Abbreviation == UKCompetentAuthorityAbbreviationType.EA &&
                deserializedcomplianceYear > EAComplianceYearCheck && scheme.OrganisationId == message.OrganisationId &&
                memberUploadsCheckAgainstNotSubmitted.Any(m => m.HasAnnualCharge) &&
                memberUploadsCheckAgainstNotSubmitted.Any(m => m.IsSubmitted);

            var producerCharges = xmlChargeBandCalculator.Calculate(message);

            totalCharges = producerCharges
                .Aggregate(totalCharges, (current, producerCharge) => current + producerCharge.Value.Amount);

            if (checkNotSubmittedNotHasAnnualCharge)
            {
                if (!checkIsSubmittedAndHasAnnualCharge && !checkNotSubmittedNotHasAnnualCharge || !checkNotSubmittedNotHasAnnualCharge && checkIsSubmittedAndHasAnnualCharge || checkNotSubmittedNotHasAnnualCharge && !checkIsSubmittedAndHasAnnualCharge)
                {
                    hasAnnualCharge = true;
                    totalCharges = totalCharges + scheme.CompetentAuthority.AnnualChargeAmount;
                }
            }

            return producerCharges;
        }

        private void CheckPreviousAnnualChargeAndReset(ProcessXmlFile message, decimal annualCharge, int deserializedcomplianceYear)
        {
            var resetMemberUploads = new List<MemberUpload>(GetMemberUploads(message, true, false, deserializedcomplianceYear));

            if (resetMemberUploads.Count > 0)
            {
                foreach (var item in resetMemberUploads)
                {   
                    if (!item.IsSubmitted)
                    { 
                        item.HasAnnualCharge = false;
                        item.DeductFromTotalCharges(annualCharge);
                    }
                }
                context.SaveChanges();
            }
        }

        private List<MemberUpload> GetMemberUploads(ProcessXmlFile message, bool hasAnnualCharge, bool isSubmitted, int deserializedcomplianceYear)
        {
            return context.MemberUploads
                    .Where(c => c.OrganisationId == message.OrganisationId)
                    .Where(c => c.ComplianceYear == deserializedcomplianceYear)
                    .Where(c => c.ComplianceYear > EAComplianceYearCheck)
                    .Where(c => c.Scheme.CompetentAuthority.Abbreviation == UKCompetentAuthorityAbbreviationType.EA)
                    .Where(c => c.HasAnnualCharge == hasAnnualCharge)
                    .Where(c => c.IsSubmitted == isSubmitted)
                    .ToList();
        }  
    }
}
