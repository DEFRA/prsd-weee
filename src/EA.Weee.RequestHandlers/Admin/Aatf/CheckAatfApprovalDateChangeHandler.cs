namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using System.Threading.Tasks;
    using AatfReturn.Internal;
    using Core.Admin;
    using Prsd.Core.Mediator;
    using Requests.Admin.Aatf;
    using Security;
    using Weee.Security;

    public class CheckAatfApprovalDateChangeHandler : IRequestHandler<CheckAatfApprovalDateChange, CanApprovalDateBeChangedFlags>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess aatfDataAccess;

        public CheckAatfApprovalDateChangeHandler(IWeeeAuthorization authorization, 
            IAatfDataAccess aatfDataAccess)
        {
            this.authorization = authorization;
            this.aatfDataAccess = aatfDataAccess;
        }

        public async Task<CanApprovalDateBeChangedFlags> HandleAsync(CheckAatfApprovalDateChange message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            var aatf = aatfDataAccess.GetDetails(message.AatfId);

            return new CanApprovalDateBeChangedFlags();
        }
    }
}
