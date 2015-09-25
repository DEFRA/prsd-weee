namespace EA.Weee.RequestHandlers.Admin
{
    using Core.Admin;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;
    using System.Threading.Tasks;

    internal class GetUserDataHandler : IRequestHandler<GetUserData, ManageUserData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetManageUserDataAccess dataAccess;

        public GetUserDataHandler(IWeeeAuthorization authorization, IGetManageUserDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<ManageUserData> HandleAsync(GetUserData query)
        {
            authorization.EnsureCanAccessInternalArea();
            
            var manageUserData = await dataAccess.GetOrganisationUser(query.OrganisationUserId) ??
                             await dataAccess.GetCompetentAuthorityUser(query.OrganisationUserId);

            return manageUserData;
        }
    }
}
