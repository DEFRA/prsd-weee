namespace EA.Weee.RequestHandlers.Admin
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
    using System.Threading.Tasks;

    public class CheckApprovalNumberIsUniqueHandler : IRequestHandler<CheckApprovalNumberIsUnique, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IFetchAatfDataAccess dataAccess;

        public CheckApprovalNumberIsUniqueHandler(
            IWeeeAuthorization authorization,
            IFetchAatfDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<bool> HandleAsync(CheckApprovalNumberIsUnique message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            Aatf existing = await dataAccess.FetchByApprovalNumber(message.ApprovalNumber);

            if (existing == null)
            {
                return false;
            }

            return true;
        }
    }
}
