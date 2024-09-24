namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DirectRegistrant;
    using System;

    public class ValidateAndGetSubmissionPayment : IRequest<SubmissionPaymentDetails>
    {
        public string PaymentReturnToken { get; private set; }

        public Guid DirectRegistrantId {get; private set; }

        public ValidateAndGetSubmissionPayment(string paymentReturnToken, Guid directRegistrantId)
        {
            Condition.Requires(paymentReturnToken).IsNotNullOrWhiteSpace();
            Condition.Requires(directRegistrantId).IsNotEqualTo(Guid.Empty);

            PaymentReturnToken = paymentReturnToken;
            DirectRegistrantId = directRegistrantId;
        }
    }
}
