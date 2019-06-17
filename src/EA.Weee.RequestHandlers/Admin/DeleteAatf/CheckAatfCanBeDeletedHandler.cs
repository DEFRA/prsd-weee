namespace EA.Weee.RequestHandlers.Admin.DeleteAatf
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.DeleteAatf;
    using EA.Weee.Security;
    using System.Threading.Tasks;

    public class CheckAatfCanBeDeletedHandler : IRequestHandler<CheckAatfCanBeDeleted, CanAatfBeDeletedFlags>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess dataAccess;

        public CheckAatfCanBeDeletedHandler(IWeeeAuthorization authorization, IAatfDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<CanAatfBeDeletedFlags> HandleAsync(CheckAatfCanBeDeleted message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            CanAatfBeDeletedFlags result = new CanAatfBeDeletedFlags();

            bool hasData = await dataAccess.DoesAatfHaveData(message.AatfId);

            if (hasData)
            {
                result |= CanAatfBeDeletedFlags.HasData;
            }

            bool orgHasMoreAatfs = await dataAccess.DoesAatfOrganisationHaveMoreAatfs(message.AatfId);

            if (orgHasMoreAatfs)
            {
                result |= CanAatfBeDeletedFlags.OrganisationHasMoreAatfs;
            }

            if (!orgHasMoreAatfs && !hasData)
            {
                result |= CanAatfBeDeletedFlags.Yes;
            }

            bool orgHasActiveUsers = await dataAccess.DoesAatfOrganisationHaveActiveUsers(message.AatfId);

            if (orgHasActiveUsers)
            {
                result |= CanAatfBeDeletedFlags.HasActiveUsers;
            }

            return result;
        }
    }
}
