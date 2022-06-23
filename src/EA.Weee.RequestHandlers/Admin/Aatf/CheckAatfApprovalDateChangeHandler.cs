namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using Core.Admin;
    using Prsd.Core.Mediator;
    using Requests.Admin.Aatf;
    using Security;
    using System.Threading.Tasks;
    using RequestHandlers.Aatf;
    using Weee.Security;

    public class CheckAatfApprovalDateChangeHandler : IRequestHandler<CheckAatfApprovalDateChange, CanApprovalDateBeChangedFlags>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IGetAatfApprovalDateChangeStatus getAatfApprovalDateChangeStatus;

        public CheckAatfApprovalDateChangeHandler(IWeeeAuthorization authorization,
            IAatfDataAccess aatfDataAccess,
            IGetAatfApprovalDateChangeStatus getAatfApprovalDateChangeStatus)
        {
            this.authorization = authorization;
            this.aatfDataAccess = aatfDataAccess;
            this.getAatfApprovalDateChangeStatus = getAatfApprovalDateChangeStatus;
        }

        public async Task<CanApprovalDateBeChangedFlags> HandleAsync(CheckAatfApprovalDateChange message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            var aatf = await aatfDataAccess.GetDetails(message.AatfId);

            return await getAatfApprovalDateChangeStatus.Validate(aatf, message.NewApprovalDate);
        }
    }
}
