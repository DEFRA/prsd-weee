namespace EA.Weee.Domain.Producer
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Domain;
    using System;

    public class PaymentSession : Entity
    {
        protected PaymentSession()
        {
        }

        public string UserId { get; set; }

        public Guid DirectRegistrantId { get; set; }

        public Guid DirectProducerSubmissionId { get; set; }

        public string PaymentId { get; set; }

        public string PaymentReference { get; set; }

        public string PaymentReturnToken { get; set; }

        public decimal Amount { get; set; }

        public PaymentState Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string UpdatedById { get; set; }

        public virtual DirectRegistrant DirectRegistrant { get; set; }

        public virtual DirectProducerSubmission DirectProducerSubmission { get; set; }

        public virtual User.User User { get; set; }

        public virtual User.User UpdatedBy { get; set; }
        
        public PaymentSession(
            string userId,
            decimal amount,
            DateTime systemDateTime,
            DirectProducerSubmission currentYearSubmission,
            DirectRegistrant directRegistrant,
            string paymentReturnToken,
            string paymentId,
            string paymentReference)
        {
            Condition.Requires(userId, nameof(userId)).IsNotNullOrWhiteSpace();
            Condition.Requires(amount, nameof(amount)).IsGreaterThan(0);
            Condition.Requires(currentYearSubmission, nameof(currentYearSubmission)).IsNotNull();
            Condition.Requires(directRegistrant, nameof(directRegistrant)).IsNotNull();
            Condition.Requires(paymentReturnToken, nameof(paymentReturnToken)).IsNotNullOrEmpty();
            Condition.Requires(paymentId, nameof(paymentId)).IsNotNullOrEmpty();
            Condition.Requires(paymentReference, nameof(paymentReference)).IsNotNullOrEmpty();

            UserId = userId;
            Amount = amount;
            CreatedAt = systemDateTime;
            DirectProducerSubmission = currentYearSubmission;
            DirectRegistrant = directRegistrant;
            PaymentReturnToken = paymentReturnToken;
            PaymentId = paymentId;
            PaymentReference = paymentReference;
            Status = PaymentState.New;
        }
    }
}
