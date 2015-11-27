namespace EA.Weee.RequestHandlers.Users.GetManageableOrganisationUsers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Organisations;
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Users.GetManageableOrganisationUsers;
    using Security;

    internal class GetManageableOrganisationUsersHandler : IRequestHandler<GetManageableOrganisationUsers, List<OrganisationUserData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IMap<OrganisationUser, OrganisationUserData> organisationUserMap;
        private readonly IGetManageableOrganisationUsersDataAccess dataAccess;

        public GetManageableOrganisationUsersHandler(IGetManageableOrganisationUsersDataAccess dataAccess, IWeeeAuthorization authorization, IMap<OrganisationUser, OrganisationUserData> organisationUserMap)
        {
            this.dataAccess = dataAccess;
            this.authorization = authorization;
            this.organisationUserMap = organisationUserMap;
        }

        public async Task<List<OrganisationUserData>> HandleAsync(GetManageableOrganisationUsers query)
        {
            authorization.EnsureOrganisationAccess(query.OrganisationId);

            var organisationUsers = await dataAccess.GetManageableUsers(query.OrganisationId);

            return organisationUsers.Select(item => organisationUserMap.Map(item)).ToList();
        }
    }
}
