namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DirectRegistrant;

    public class ValidateAndGetSubmissionPayment : IRequest<SubmissionPaymentDetails>
    {
        public string PaymentReturnToken { get; private set; }

        public ValidateAndGetSubmissionPayment(string paymentReturnToken)
        {
            Condition.Requires(paymentReturnToken).IsNotNullOrWhiteSpace();

            PaymentReturnToken = paymentReturnToken;
        }
    }
}
