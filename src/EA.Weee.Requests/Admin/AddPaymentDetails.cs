namespace EA.Weee.Requests.Admin
{
    using EA.Weee.Core.PaymentDetails;
    using Prsd.Core.Mediator;
    using System;

    public class AddPaymentDetails : IRequest<OfflinePaymentDetails>
    {
        public Guid DirectProducerSubmissionId { get; set; }

        public string PaymentMethod { get; set; }

        public DateTime? PaymentRecievedDate { get; set; }

        public string PaymentDetailsDescription { get; set; }

        public AddPaymentDetails(string paymentMethod, DateTime? paymentRecievedDate, string paymentDetails, Guid directProducerSubmissionId)
        {
            PaymentMethod = paymentMethod;
            PaymentRecievedDate = paymentRecievedDate;
            PaymentDetailsDescription = paymentDetails;
            DirectProducerSubmissionId = directProducerSubmissionId;
        }
    }
}
