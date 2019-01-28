namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public partial class Return : Entity
    {
        public Return(Guid returnid, int complianceyear, int period, int status, Operator aatfOperator)
        {
            ReturnId = returnid;
            Operator = aatfOperator;
            ComplianceYear = complianceyear;
            Period = period;
            Status = status;
        }

        public Guid ReturnId { get; private set; }

        public Guid OperatorId { get; private set; }

        public int ComplianceYear { get; private set; }

        public int Period { get; private set; }

        public int Status { get; private set; }

        public virtual Operator Operator { get; private set; }
}
}
