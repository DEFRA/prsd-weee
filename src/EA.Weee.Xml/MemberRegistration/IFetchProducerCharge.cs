namespace EA.Weee.Xml.MemberRegistration
{
    using Domain.Lookup;
    using System;
    using System.Threading.Tasks;

    public interface IFetchProducerCharge
    {
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
    }
}