namespace EA.Weee.RequestHandlers.Users
{
    using System;
    using System.Threading.Tasks;
    using Core.Organisations;
    using Domain.Organisation;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mapper;
    using Requests.Users;

    internal class GetOrganisationUserHandler : IRequestHandler<GetOrganisationUser, OrganisationUserData>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly IMap<OrganisationUser, OrganisationUserData> organisationUserMap;

        public GetOrganisationUserHandler(WeeeContext context, IWeeeAuthorization authorization, IMap<OrganisationUser, OrganisationUserData> organisationUserMap)
        {
            this.context = context;
            this.authorization = authorization;
            this.organisationUserMap = organisationUserMap;
        }

        public async Task<OrganisationUserData> HandleAsync(GetOrganisationUser query)
        {
            OrganisationUser organisationUser = await context.OrganisationUsers.FindAsync(query.OrganisationUserId);

            if (organisationUser == null)
            {
                string message = string.Format(
                    "No organisation user was found with ID \"{0}\".",
                    query.OrganisationUserId);
                throw new Exception(message);
            }
            
            authorization.EnsureOrganisationAccess(organisationUser.OrganisationId);

            return organisationUserMap.Map(organisationUser);
        }
    }
}
