namespace EA.Weee.RequestHandlers.Users.UpdateOrganisationUserStatus
{
    using System;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using Mappings;
    using Prsd.Core.Mediator;
    using Requests.Users;
    using Security;

    internal class UpdateOrganisationUserStatusHandler : IRequestHandler<UpdateOrganisationUserStatus, int>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IUpdateOrganisationUserStatusDataAccess dataAccess;

        public UpdateOrganisationUserStatusHandler(IWeeeAuthorization authorization, IUpdateOrganisationUserStatusDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<int> HandleAsync(UpdateOrganisationUserStatus query)
        {
            var organisationUser = await dataAccess.GetOrganisationUser(query.OrganisationUserId);

            if (organisationUser == null)
            {
                throw new Exception(string.Format("No organisation user was found with ID \"{0}\".", query.OrganisationUserId));
            }

            authorization.EnsureInternalOrOrganisationAccess(organisationUser.OrganisationId);

            return await dataAccess.ChangeOrganisationUserStatus(organisationUser, query.UserStatus);
        }
    }
}
