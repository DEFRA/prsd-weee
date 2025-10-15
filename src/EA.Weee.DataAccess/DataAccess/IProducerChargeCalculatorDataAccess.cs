namespace EA.Weee.DataAccess.DataAccess
{
    using Domain.Lookup;
    using System;
    using System.Threading.Tasks;

    public interface IProducerChargeCalculatorDataAccess
    {
        /// <summary>
        /// Asynchronously retrieves the charge band amount for the specified charge band type.
        /// </summary>
        /// <param name="chargeBandType">The type of charge band for which the amount is to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the  <see
        /// cref="ChargeBandAmount"/> associated with the specified charge band type.</returns>
        Task<ChargeBandAmount> GetChargeBandAmountAsyncLegacy(ChargeBand chargeBandType);

        /// <summary>
        /// Fetches the complete charge band amount record based on criteria and effective date rules.
        /// This includes all charge metadata like charge band, amount, and effective dates.
        /// </summary>
        /// <param name="competentAuthority">The competent authority</param>
        /// <param name="vatRegistered">VAT registration status</param>
        /// <param name="annualTurnoverBand">Annual turnover band</param>
        /// <param name="eeePlacedOnMarketBand">EEE placed on market band</param>
        /// <param name="complianceYear">Compliance year</param>
        /// <param name="asOfUtc">As-of date for effective date filtering</param>
        /// <returns>The complete ChargeBandAmount record</returns>
        Task<ChargeBandAmount> GetChargeBandAmountAsync(
            CompetentAuthorityType competentAuthority,
            bool vatRegistered,
            AnnualTurnoverBand annualTurnoverBand,
            EEEPlacedOnMarketBand eeePlacedOnMarketBand,
            int complianceYear,
            DateTime asOfUtc);
    }
}
