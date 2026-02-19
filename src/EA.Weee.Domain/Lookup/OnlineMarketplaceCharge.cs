namespace EA.Weee.Domain.Lookup
{
    using System;

    /// <summary>
    /// Provides the Online Marketplace (OMP) registration charge amount in GBP
    /// based on effective dates. This allows fee changes to be pre-loaded and
    /// automatically applied when the effective date is reached.
    /// </summary>
    public class OnlineMarketplaceCharge
    {
        public Guid Id { get; private set; }

        public decimal Amount { get; private set; }

        public DateTime EffectiveFrom { get; private set; }

        public OnlineMarketplaceCharge(Guid id, decimal amount, DateTime effectiveFrom)
        {
            Id = id;
            Amount = amount;
            EffectiveFrom = effectiveFrom;
        }

        /// <summary>
        /// This constructor should only be used by Entity Framework.
        /// </summary>
        protected OnlineMarketplaceCharge()
        {
        }
    }
}