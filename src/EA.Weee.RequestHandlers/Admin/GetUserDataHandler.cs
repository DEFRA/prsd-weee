﻿namespace EA.Weee.RequestHandlers.Admin
{
    using Core.Admin;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;
    using System.Threading.Tasks;
    using Weee.Security;

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
                manageUserData.CanManageRoleAndStatus = false;
            }

            manageUserData.CanEditUser = authorization.CheckUserInRole(Roles.InternalAdmin);

            return manageUserData;
        }
    }
}
