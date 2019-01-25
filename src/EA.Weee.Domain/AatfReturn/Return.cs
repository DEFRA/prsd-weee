namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public partial class Return : Entity
    {
        public Return(Guid returnid, Guid operatorid, int complianceyear, int period, int status)
        {
            ReturnId = returnid;
            OperatorId = operatorid;
            ComplianceYear = complianceyear;
            Period = period;
            Status = status;
        }

        public Guid ReturnId { get; private set; }

        public Guid OperatorId { get; private set; }

        public int ComplianceYear { get; private set; }

        public int Period { get; private set; }

        public int Status { get; private set; }
    }
}
