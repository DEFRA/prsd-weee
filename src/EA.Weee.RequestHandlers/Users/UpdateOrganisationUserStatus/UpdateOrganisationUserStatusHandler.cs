namespace EA.Weee.RequestHandlers.Users.UpdateOrganisationUserStatus
{
    using System;
    using System.Threading.Tasks;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Users;
    using Security;

    internal class UpdateOrganisationUserStatusHandler : IRequestHandler<UpdateOrganisationUserStatus, int>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IUpdateOrganisationUserStatusDataAccess dataAccess;
        private readonly IUserContext userContext;

        public UpdateOrganisationUserStatusHandler(IUserContext userContext, IWeeeAuthorization authorization, IUpdateOrganisationUserStatusDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.userContext = userContext;
        }

        public async Task<int> HandleAsync(UpdateOrganisationUserStatus query)
        {
            var organisationUser = await dataAccess.GetOrganisationUser(query.OrganisationUserId);

            if (organisationUser == null)
            {
                throw new Exception(string.Format("No organisation user was found with ID \"{0}\".", query.OrganisationUserId));
            }

            authorization.EnsureInternalOrOrganisationAccess(organisationUser.OrganisationId);

            if (userContext != null && userContext.UserId.ToString() == organisationUser.UserId)
            {
                throw new InvalidOperationException(string.Format("Error for user with Id '{0}': Users cannot change their own status", userContext.UserId));
            }

            return await dataAccess.ChangeOrganisationUserStatus(organisationUser, query.UserStatus);
        }
    }
}
