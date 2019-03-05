namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.RequestHandlers.Scheme.MemberRegistration;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Scheme.MemberRegistration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CheckForAnnualCharge : ITotalChargeCalculatorDataAccess
    {
        private const int EAComplianceYearCheck = 2018;
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;

        public CheckForAnnualCharge(WeeeContext context, IWeeeAuthorization authorization)
        {
            this.context = context;
            this.authorization = authorization;
        }
        public bool CheckForNotSubmitted(ProcessXmlFile message, Scheme scheme, int deserializedcomplianceYear, List<MemberUpload> memberUploadsCheckAgainstNotSubmitted)
        {
            return scheme.CompetentAuthority.Abbreviation == UKCompetentAuthorityAbbreviationType.EA &&
                            deserializedcomplianceYear > EAComplianceYearCheck && scheme.OrganisationId == message.OrganisationId &&
                            memberUploadsCheckAgainstNotSubmitted.Any(m => !m.HasAnnualCharge) &&
                            memberUploadsCheckAgainstNotSubmitted.Any(m => !m.IsSubmitted);
        }

        public bool CheckSchemeHasAnnualCharge(Scheme scheme, int deserializedcomplianceYear)
        {
            return context.MemberUploads.Any(m => m.HasAnnualCharge 
                                                  && m.Scheme.OrganisationId == scheme.OrganisationId 
                                                  && m.ComplianceYear == deserializedcomplianceYear
                                                  && m.IsSubmitted);
        }

        public List<MemberUpload> GetMemberUploads(ProcessXmlFile message, bool hasAnnualCharge, bool isSubmitted, int deserializedcomplianceYear)
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
