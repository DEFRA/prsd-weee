namespace EA.Weee.RequestHandlers.Users
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Organisations;
    using Domain.Organisation;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mapper;
    using Requests.Users;

    internal class GetUsersByOrganisationIdHandler : IRequestHandler<GetUsersByOrganisationId, List<OrganisationUserData>>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly IMap<OrganisationUser, OrganisationUserData> organisationUserMap;

        public GetUsersByOrganisationIdHandler(WeeeContext context, IWeeeAuthorization authorization, IMap<OrganisationUser, OrganisationUserData> organisationUserMap)
        {
            this.context = context;
            this.authorization = authorization;
            this.organisationUserMap = organisationUserMap;
        }

        public async Task<List<OrganisationUserData>> HandleAsync(GetUsersByOrganisationId query)
        {
            authorization.EnsureOrganisationAccess(query.OrganisationId);

            var organisationUsers =
                await context.OrganisationUsers.Where(ou => ou.OrganisationId == query.OrganisationId).ToListAsync();

            return organisationUsers.Select(item => organisationUserMap.Map(item)).ToList();
        }
    }
}
