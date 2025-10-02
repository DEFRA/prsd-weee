namespace EA.Weee.Domain.Lookup
{
    using System;

    /// <summary>
    /// Provides the amount in GBP that a producer in the specified
    /// charge band will be charged based on jurisdictional rules, compliance year, and effective dates.
    /// </summary>
    public class ChargeBandAmount
    {
        public Guid Id { get; private set; }

        public ChargeBand ChargeBand { get; private set; }

        public CompetentAuthorityType CompetentAuthority { get; private set; }

        public bool VatRegistered { get; private set; }

        public AnnualTurnoverBand AnnualTurnoverBand { get; private set; }

        public EEEPlacedOnMarketBand EEEPlacedOnMarketBand { get; private set; }

        public int ComplianceYear { get; private set; }

        public decimal Amount { get; private set; }

        public DateTime EffectiveFrom { get; private set; }

        public ChargeBandAmount(
            Guid id, 
            ChargeBand chargeBand, 
            CompetentAuthorityType competentAuthority,
            bool vatRegistered,
            AnnualTurnoverBand annualTurnoverBand,
            EEEPlacedOnMarketBand eeePlacedOnMarketBand,
            int complianceYear,
            decimal amount,
            DateTime effectiveFrom)
        {
            Id = id;
            ChargeBand = chargeBand;
            CompetentAuthority = competentAuthority;
            VatRegistered = vatRegistered;
            AnnualTurnoverBand = annualTurnoverBand;
            EEEPlacedOnMarketBand = eeePlacedOnMarketBand;
            ComplianceYear = complianceYear;
            Amount = amount;
            EffectiveFrom = effectiveFrom;
        }

        /// <summary>
        /// This constructor should only be used by Entity Framework.
        /// </summary>
        public ChargeBandAmount()
        {
        }
    }
}
