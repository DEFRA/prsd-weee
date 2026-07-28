namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using EA.Weee.DataAccess;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
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
            var now = DateTime.UtcNow;

            var annualCharge = await context.AnnualChargesByYear
                .Where(a => a.CompetentAuthorityId == competentAuthorityId)
                .Where(a => a.ComplianceYear == complianceYear)
                .Where(a => a.EffectiveFrom == null || a.EffectiveFrom <= now)
                .OrderByDescending(a => a.EffectiveFrom)
                .FirstOrDefaultAsync();

            return annualCharge?.AnnualChargeAmount;
        }
    }
}
