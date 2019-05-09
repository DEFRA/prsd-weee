namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using DataReturns;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using User;

    public partial class Return : Entity
    {
        protected Return()
        {
        }

        public Return(Operator aatfOperator, Quarter quarter, string createdBy)
        {
            Guard.ArgumentNotNull(() => aatfOperator, aatfOperator);
            Guard.ArgumentNotNull(() => quarter, quarter);
            Guard.ArgumentNotNullOrEmpty(() => createdBy, createdBy);

            Operator = aatfOperator;
            Quarter = quarter;
            ReturnStatus = ReturnStatus.Created;
            CreatedById = createdBy;
            CreatedDate = SystemTime.UtcNow;
        }

        public virtual void UpdateSubmitted(string submittedBy)
        {
            Guard.ArgumentNotNullOrEmpty(() => submittedBy, submittedBy);

            if (ReturnStatus != ReturnStatus.Created)
            {
                throw new InvalidOperationException("Return status must be Created to transition to Submitted");
            }

            SubmittedById = submittedBy;
            SubmittedDate = SystemTime.UtcNow;
            ReturnStatus = ReturnStatus.Submitted;
        }

        public virtual Quarter Quarter { get; private set; }

        public Guid OperatorId { get; set; }

        public virtual ReturnStatus ReturnStatus { get; set; }

        public virtual Operator Operator { get; private set; }

        public virtual DateTime CreatedDate { get; private set; }

        public virtual DateTime? SubmittedDate { get; set; }

        public virtual string CreatedById { get; private set; }
        
        public virtual string SubmittedById { get; set; }

        public virtual User CreatedBy { get; set; }

        public virtual User SubmittedBy { get; set; }
    }
}