namespace EA.Weee.Requests.Scheme
{
    using EA.Prsd.Core.Mediator;

    public class VerifyApprovalNumberExists : IRequest<bool>
    {
        public string ApprovalNumber { get; private set; }

        public VerifyApprovalNumberExists(string approvalNumber)
        {
            ApprovalNumber = approvalNumber;
        }
    }
}
