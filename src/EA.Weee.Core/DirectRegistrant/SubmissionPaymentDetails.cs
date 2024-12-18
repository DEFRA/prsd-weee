namespace EA.Weee.Core.DirectRegistrant
{
    using System;

    public class SubmissionPaymentDetails
    {
        public string ErrorMessage { get; set; }

        public Guid DirectRegistrantId { get; set; }

        public string PaymentId { get; set; }

        public string PaymentReference { get; set; }

        public Guid PaymentSessionId { get; set; }

        public bool PaymentFinished { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
    }
}
