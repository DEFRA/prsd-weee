namespace EA.Weee.RequestHandlers.Users
{
    using System.Data.Entity;
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
            authorization.EnsureOrganisationAccess(query.OrganisationId);

            var organisationUsers =
                await context.OrganisationUsers.SingleOrDefaultAsync(ou => ou.OrganisationId == query.OrganisationId
                    && ou.UserId == query.UserId.ToString());

            return organisationUserMap.Map(organisationUsers);
        }
    }
}
