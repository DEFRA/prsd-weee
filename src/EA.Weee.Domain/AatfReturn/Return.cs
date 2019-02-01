namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using DataReturns;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public partial class Return : Entity
    {
        protected Return()
        {
        }
        public Return(Operator aatfOperator, Quarter quarter)
        {
            Guard.ArgumentNotNull(() => aatfOperator, aatfOperator);
            Guard.ArgumentNotNull(() => quarter, quarter);

            Operator = aatfOperator;
            Quarter = quarter;
            Status = 1;
        }

        public virtual Quarter Quarter { get; private set; }

        public Guid OperatorId { get; private set; }

        public int Status { get; private set; }

        public virtual Operator Operator { get; private set; }
}
}
