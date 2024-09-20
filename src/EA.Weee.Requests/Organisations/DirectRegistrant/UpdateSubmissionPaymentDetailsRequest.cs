namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DirectRegistrant;
    using System;

    public class UpdateSubmissionPaymentDetailsRequest : IRequest<bool>
    {
        public Guid DirectRegistrantId { get; private set; }

        public PaymentStatus PaymentStatus { get; private set; }

        public Guid PaymentSessionId { get; private set; }

        public bool IsFinalState { get; private set; }

        public UpdateSubmissionPaymentDetailsRequest(Guid directRegistrantId, PaymentStatus paymentStatus, Guid paymentSessionId, bool isFinalState)
        {
            Condition.Requires(directRegistrantId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(paymentSessionId).IsNotEqualTo(Guid.Empty);

            DirectRegistrantId = directRegistrantId;
            PaymentStatus = paymentStatus;
            PaymentSessionId = paymentSessionId;
            IsFinalState = isFinalState;
        }
    }
}
