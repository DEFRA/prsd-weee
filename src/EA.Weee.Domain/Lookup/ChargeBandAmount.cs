namespace EA.Weee.Domain.Lookup
{
    using System;

    /// <summary>
    /// Provides the amount in GBP that a producer in the specified
    /// charge band will be charged.
    /// </summary>
    public class ChargeBandAmount
    {
        public Guid Id { get; private set; }

        public ChargeBand ChargeBand { get; private set; }

        public decimal Amount { get; private set; }

        public ChargeBandAmount(Guid id, ChargeBand chargeBand, decimal amount)
        {
            Id = id;
            ChargeBand = chargeBand;
            Amount = amount;
        }

        /// <summary>
        /// This constructor should only be used by Entity Framework.
        /// </summary>
        public ChargeBandAmount()
        {
        }
    }
}
