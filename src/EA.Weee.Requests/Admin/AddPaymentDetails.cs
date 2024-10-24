namespace EA.Weee.Requests.Admin
{
    using EA.Weee.Core.PaymentDetails;
    using Prsd.Core.Mediator;
    using System;

    public class AddPaymentDetails : IRequest<ManualPaymentResult>
    {
        public Guid DirectProducerSubmissionId { get; set; }

        public string PaymentMethod { get; set; }

        public DateTimeInput PaymentRecievedDate { get; set; }

        public string PaymentDetailsDescription { get; set; }

        public AddPaymentDetails(string paymentMethod, DateTimeInput paymentRecievedDate, string paymentDetails, Guid directProducerSubmissionId)
        {
            PaymentMethod = paymentMethod;
            PaymentRecievedDate = paymentRecievedDate;
            PaymentDetailsDescription = paymentDetails;
            DirectProducerSubmissionId = directProducerSubmissionId;
        }
    }
}
