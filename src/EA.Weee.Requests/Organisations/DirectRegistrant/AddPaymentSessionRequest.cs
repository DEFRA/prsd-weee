namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;
    using System;
    using EA.Weee.Core.Organisations;

    public class AddPaymentSessionRequest : IRequest<bool>
    {
        public Guid DirectRegistrantId { get; private set; }

        public string PaymentReference {get; private set; }

        public string PaymentReturnToken { get; private set; }

        public string PaymentId {get; private set; }

        public decimal Amount { get; private set; }

        public AddPaymentSessionRequest(Guid directRegistrantId, string paymentReference, string paymentReturnToken, string paymentId, decimal amount)
        {
            Condition.Requires(directRegistrantId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(paymentReference).IsNotNullOrWhiteSpace();
            Condition.Requires(paymentReturnToken).IsNotNullOrWhiteSpace();
            Condition.Requires(paymentId).IsNotNullOrWhiteSpace();
            Condition.Requires(amount).IsGreaterThan(0);

            DirectRegistrantId = directRegistrantId;
            PaymentId = paymentId;
            PaymentReference = paymentReference;    
            PaymentReturnToken = paymentReturnToken;
            Amount = amount;
        }
    }
}
