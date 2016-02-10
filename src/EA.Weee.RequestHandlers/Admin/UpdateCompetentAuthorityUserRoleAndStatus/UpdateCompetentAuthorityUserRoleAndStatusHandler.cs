namespace EA.Weee.RequestHandlers.Admin.UpdateCompetentAuthorityUserRoleAndStatus
{
    using System;
    using System.Threading.Tasks;
    using Core.Security;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;

    internal class UpdateCompetentAuthorityUserRoleAndStatusHandler : IRequestHandler<UpdateCompetentAuthorityUserRoleAndStatus, Guid>
    {
        private readonly IUpdateCompetentAuthorityUserRoleAndStatusDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly IUserContext userContext;

        public UpdateCompetentAuthorityUserRoleAndStatusHandler(IUserContext userContext, IUpdateCompetentAuthorityUserRoleAndStatusDataAccess dataAccess, IWeeeAuthorization authorization)
        {
            this.userContext = userContext;
            this.dataAccess = dataAccess;
            this.authorization = authorization;
        }

        public async Task<Guid> HandleAsync(UpdateCompetentAuthorityUserRoleAndStatus query)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            var competentAuthorityUser = await dataAccess.GetCompetentAuthorityUser(query.Id);

            if (competentAuthorityUser == null)
            {
                string message = string.Format(
                    "No competent authority user was found with ID \"{0}\".",
                    query.Id);
                throw new InvalidOperationException(message);
            }

            if (userContext != null &&
                userContext.UserId.ToString() == competentAuthorityUser.UserId)
            {
                throw new InvalidOperationException(string.Format("Error for user with Id '{0}': Users cannot change their own status or role", userContext.UserId));
            }

            var role = await dataAccess.GetRoleOrDefaultAsync(query.RoleName);
            if (role == null)
            {
                throw new InvalidOperationException(string.Format("Invalid role name {0}", query.RoleName));
            }

            await dataAccess.UpdateUserRoleAndStatus(competentAuthorityUser, role, query.UserStatus);

            return competentAuthorityUser.CompetentAuthorityId;
        }
    }
}
