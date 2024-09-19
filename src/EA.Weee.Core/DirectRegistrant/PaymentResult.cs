namespace EA.Weee.Core.DirectRegistrant
{
    using System;

    public class PaymentResult
    {
        public string PaymentReference { get; set; }

        public Guid DirectRegistrantId { get; set; }
    }
}
