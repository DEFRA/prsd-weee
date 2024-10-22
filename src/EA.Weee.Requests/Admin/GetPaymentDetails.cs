namespace EA.Weee.Requests.Admin
{
    using EA.Weee.Core.PaymentDetails;
    using Prsd.Core.Mediator;
    using System;

    public class GetPaymentDetails : IRequest<OfflinePaymentDetails>
    {
        public Guid DirectProducerSubmissionId { get; set; }

        public GetPaymentDetails(Guid directProducerSubmissionId)
        {
            DirectProducerSubmissionId = directProducerSubmissionId;
        }
    }
}
