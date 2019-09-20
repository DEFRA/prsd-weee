namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using DataAccess;
    using Domain.Scheme;
    using System.Linq;

    public class TotalChargeCalculatorDataAccess : ITotalChargeCalculatorDataAccess
    {
        private readonly WeeeContext context;

        public TotalChargeCalculatorDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public bool CheckSchemeHasAnnualCharge(Scheme scheme, int deserializedcomplianceYear)
        {
            return context.MemberUploads.Any(m => m.HasAnnualCharge
                                                  && m.Scheme.OrganisationId == scheme.OrganisationId
                                                  && m.ComplianceYear == deserializedcomplianceYear
                                                  && m.IsSubmitted);
        }
    }
}
