namespace EA.Weee.Requests.Admin.Aatf
{
    using System;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class CheckAatfApprovalDateChange : IRequest<CanApprovalDateBeChangedFlags>
    {
        public Guid AatfId { get; private set; }

        public DateTime NewApprovalDate { get; private set; }

        public CheckAatfApprovalDateChange(Guid aatfId, DateTime newApprovalDate)
        {
            AatfId = aatfId;
            NewApprovalDate = newApprovalDate;
        }
    }
}
