namespace EA.Weee.RequestHandlers.Admin
{
    using System.Threading.Tasks;
    using Core.Admin;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;

    internal class GetUserDataHandler : IRequestHandler<GetUserData, ManageUserData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetManageUserDataAccess dataAccess;
        private readonly IUserContext userContext;

        public GetUserDataHandler(IUserContext userContext, IWeeeAuthorization authorization, IGetManageUserDataAccess dataAccess)
        {
            this.userContext = userContext;
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<ManageUserData> HandleAsync(GetUserData query)
        {
            authorization.EnsureCanAccessInternalArea();
            
            var manageUserData = await dataAccess.GetOrganisationUser(query.OrganisationUserId) ??
                             await dataAccess.GetCompetentAuthorityUser(query.OrganisationUserId);

            if (manageUserData != null 
                && userContext != null 
                && userContext.UserId.ToString() == manageUserData.UserId)
            {
                manageUserData.CanManageStatus = false;
            }

            return manageUserData;
        }
    }
}
