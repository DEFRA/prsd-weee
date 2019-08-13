namespace EA.Weee.Requests.Admin
{
    using EA.Weee.Core.AatfReturn;
    using Prsd.Core.Mediator;

    public class CheckApprovalNumberIsUnique : IRequest<bool>
    {
        public string ApprovalNumber { get; private set; }

        public CheckApprovalNumberIsUnique(string approvalNumber)
        {
            this.ApprovalNumber = approvalNumber;
        }
    }
}
