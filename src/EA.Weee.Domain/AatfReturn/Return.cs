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

        public Return(Operator aatfOperator, Quarter quarter, ReturnStatus returnStatus, string createdBy)
        {
            Guard.ArgumentNotNull(() => aatfOperator, aatfOperator);
            Guard.ArgumentNotNull(() => quarter, quarter);
            Guard.ArgumentNotNull(() => returnStatus, returnStatus);
            Guard.ArgumentNotNullOrEmpty(() => createdBy, createdBy);

            Operator = aatfOperator;
            Quarter = quarter;
            ReturnStatus = returnStatus;
            CreatedBy = createdBy;
            CreatedDate = DateTime.Now;
        }

        public virtual Quarter Quarter { get; private set; }

        public Guid OperatorId { get; set; }

        public virtual ReturnStatus ReturnStatus { get; private set; }

        public virtual Operator Operator { get; private set; }

        public virtual DateTime CreatedDate { get; private set; }

        public virtual DateTime? SubmittedDate { get; set; }

        public virtual string CreatedBy { get; private set; }

        public virtual string SubmittedBy { get; set; }
    }
}