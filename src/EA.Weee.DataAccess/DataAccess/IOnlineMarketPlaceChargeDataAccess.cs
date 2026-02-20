namespace EA.Weee.DataAccess.DataAccess
{
    using Domain.Lookup;
    using System;
    using System.Threading.Tasks;

    public interface IOnlineMarketplaceChargeDataAccess
    {
        /// <summary>
        /// Retrieves the Online Marketplace charge amount that is effective as of the specified date
        /// for the given competent authority.
        /// Returns the most recent charge where EffectiveFrom is on or before the asOfUtc date.
        /// </summary>
        /// <param name="competentAuthority">The competent authority type</param>
        /// <param name="asOfUtc">The date to check against effective dates</param>
        /// <returns>The applicable OnlineMarketplaceCharge, or null if none found</returns>
        Task<OnlineMarketplaceCharge> GetChargeAmountAsync(CompetentAuthorityType competentAuthority, DateTime asOfUtc);
    }
}