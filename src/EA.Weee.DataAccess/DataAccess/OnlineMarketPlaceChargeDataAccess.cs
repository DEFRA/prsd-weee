namespace EA.Weee.DataAccess.DataAccess
{
    using Domain.Lookup;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class OnlineMarketplaceChargeDataAccess : IOnlineMarketplaceChargeDataAccess
    {
        private readonly WeeeContext context;

        public OnlineMarketplaceChargeDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Retrieves the Online Marketplace charge amount that is effective as of the specified date.
        /// Returns the most recent charge where EffectiveFrom is on or before the asOfUtc date.
        /// </summary>
        public async Task<OnlineMarketplaceCharge> GetChargeAmountAsync(CompetentAuthorityType competentAuthority, DateTime asOfUtc)
        {
            return await context.OnlineMarketplaceCharges
                    .Where(o => o.CompetentAuthority == competentAuthority)
                    .Where(o => o.EffectiveFrom <= asOfUtc)
                    .OrderByDescending(o => o.EffectiveFrom)
                    .FirstOrDefaultAsync();
        }
    }
}