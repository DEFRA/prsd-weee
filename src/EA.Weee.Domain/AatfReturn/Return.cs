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

        public Return(Operator aatfOperator, Quarter quarter, ReturnStatus returnStatus)
        {
            Guard.ArgumentNotNull(() => aatfOperator, aatfOperator);
            Guard.ArgumentNotNull(() => quarter, quarter);
            Guard.ArgumentNotNull(() => returnStatus, returnStatus);

            Operator = aatfOperator;
            Quarter = quarter;
            ReturnStatus = returnStatus;
        }

        public virtual Quarter Quarter { get; private set; }

        public Guid OperatorId { get; private set; }

        public virtual ReturnStatus ReturnStatus { get; private set; }

        public virtual Operator Operator { get; private set; }

        public virtual DateTime CreateDate { get; private set; }

        public virtual DateTime SubmittedDate { get; private set; }

        public virtual User CreatedBy { get; private set; }

        public virtual User SubmittedBy { get; private set; }
    }
}