namespace EA.Weee.Xml.MemberRegistration
{
    using Domain.Lookup;
    using System;
    using System.Threading.Tasks;

    public interface IFetchProducerCharge
    {
        /// <summary>
        /// Retrieves the charge amount for the specified charge band LEGACY.
        /// </summary>
        /// <param name="chargeBand">The charge band for which the charge amount is to be retrieved.</param>
        /// <returns>A <see cref="ProducerCharge"/> representing the charge amount for the specified charge band.</returns>
        Task<ProducerCharge> GetChargeBandAmountAsyncLegacy(ChargeBand chargeBand);
        
        /// <summary>
        /// Enhanced method for fetching complete charge band amount record based on criteria and effective date rules.
        /// Returns the full ChargeBandAmount object with all metadata including charge band and amount.
        /// </summary>
        Task<ChargeBandAmount> GetChargeBandAmountAsync(
            CompetentAuthorityType competentAuthority,
            bool vatRegistered,
            AnnualTurnoverBand annualTurnoverBand,
            EEEPlacedOnMarketBand eeePlacedOnMarketBand,
            int complianceYear,
            DateTime asOfUtc);

        /// <summary>
        /// Retrieves the Online Marketplace charge for the specified competent authority and date.
        /// </summary>
        /// <param name="competentAuthority">The competent authority type</param>
        /// <param name="asOfUtc">The date to check against effective dates</param>
        /// <returns>The charge amount, or null if no charge is configured</returns>
        Task<decimal?> GetOnlineMarketplaceChargeAsync(CompetentAuthorityType competentAuthority, DateTime asOfUtc);
    }
}