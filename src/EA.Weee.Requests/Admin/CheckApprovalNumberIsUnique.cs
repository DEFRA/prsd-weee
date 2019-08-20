namespace EA.Weee.Requests.Admin
{
    using Prsd.Core.Mediator;

    public class CheckApprovalNumberIsUnique : IRequest<bool>
    {
        public string ApprovalNumber { get; private set; }

        public int? ComplianceYear { get; private set; }

        public CheckApprovalNumberIsUnique(string approvalNumber, int? complianceYear = null)
        {
            ApprovalNumber = approvalNumber;
            ComplianceYear = complianceYear;
        }
    }
}
