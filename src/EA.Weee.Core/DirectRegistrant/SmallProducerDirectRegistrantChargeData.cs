namespace EA.Weee.Core.DirectRegistrant
{
    using System;

    public class SmallProducerDirectRegistrantChargeData
    {
        public Guid Id { get; set; }

        public int ComplianceYear { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public decimal ChargeAmount { get; set; }
    }
}
