namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;
    using System;
    using EA.Weee.Core.Organisations;

    public class UpdateSubmissionPaymentDetailsRequest : IRequest<bool>
    {
        public Guid DirectRegistrantId { get; private set; }

        public string PaymentReference {get; private set; }

        public string PaymentReturnToken { get; private set; }

        public string PaymentId {get; private set; }

        public UpdateSubmissionPaymentDetailsRequest(Guid directRegistrantId, string paymentReference, string paymentReturnToken, string paymentId)
        {
            Condition.Requires(directRegistrantId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(paymentReference).IsNotNullOrWhiteSpace();
            Condition.Requires(paymentReturnToken).IsNotNullOrWhiteSpace();
            Condition.Requires(paymentId).IsNotNullOrWhiteSpace();

            DirectRegistrantId = directRegistrantId;
            PaymentId = paymentId;
            PaymentReference = paymentReference;    
            PaymentReturnToken = paymentReturnToken;
        }
    }
}
