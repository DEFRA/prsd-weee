namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using EA.Weee.DataAccess;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class AnnualChargeDataAccess : IAnnualChargeDataAccess
    {
        private readonly WeeeContext context;

        public AnnualChargeDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<decimal?> GetAnnualChargeForComplianceYear(Guid competentAuthorityId, int complianceYear)
        {     
            var annualCharge = await context.AnnualChargesByYear
                .Where(a => a.CompetentAuthorityId == competentAuthorityId)
                .Where(a => a.ComplianceYear == complianceYear)
                .OrderByDescending(a => a.EffectiveFrom)
                .FirstOrDefaultAsync();

            if (annualCharge != null)
            {
                return annualCharge.AnnualChargeAmount;
            }

            // Fallback to the legacy AnnualChargeAmount from CompetentAuthority table
            var competentAuthority = await context.UKCompetentAuthorities
                .Where(c => c.Id == competentAuthorityId)
                .FirstOrDefaultAsync();

            return competentAuthority?.AnnualChargeAmount;
        }
    }
}
