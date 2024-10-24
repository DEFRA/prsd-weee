namespace EA.Weee.Domain.Producer
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Domain;
    using System;

    public class PaymentSession : Entity
    {
        public PaymentSession()
        {
        }

        public virtual string UserId { get; set; }

        public virtual Guid DirectRegistrantId { get; set; }

        public virtual Guid DirectProducerSubmissionId { get; set; }

        public virtual string PaymentId { get; set; }

        public virtual string PaymentReference { get; set; }

        public virtual string PaymentReturnToken { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual PaymentState Status { get; set; }

        public virtual bool InFinalState { get; set; }

        public virtual DateTime CreatedAt { get; set; }

        public virtual DateTime? UpdatedAt { get; set; }

        public virtual DateTime? LastProcessedAt { get; set; }

        public virtual string UpdatedById { get; set; }

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
