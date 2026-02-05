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

            return annualCharge?.AnnualChargeAmount;
        }
    }
}
