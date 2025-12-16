namespace EA.Weee.Domain.Lookup
{
    using System;

    public class DirectRegistrantCharge
    {
        public Guid Id { get; private set; }

        public int ComplianceYear { get; private set; }

        public DateTime EffectiveFrom { get; private set; }

        public decimal ChargeAmount { get; private set; }

        public bool IsEngland { get; private set; }

        /// <summary>
        /// This constructor should only be used by Entity Framework.
        /// </summary>
        public DirectRegistrantCharge()
        {
        }
    }
}
