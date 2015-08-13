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
    using Prsd.Core.Mapper;
    using Requests.Users;

    internal class GetUsersByOrganisationIdHandler : IRequestHandler<GetUsersByOrganisationId, List<OrganisationUserData>>
    {
        private readonly WeeeContext context;
        private readonly IMap<OrganisationUser, OrganisationUserData> organisationUserMap;

        public GetUsersByOrganisationIdHandler(WeeeContext context, IMap<OrganisationUser, OrganisationUserData> organisationUserMap)
        {
            this.context = context;
            this.organisationUserMap = organisationUserMap;
        }

        public async Task<List<OrganisationUserData>> HandleAsync(GetUsersByOrganisationId query)
        {
            var organisation = await context.Organisations.SingleOrDefaultAsync(o => o.Id == query.OrganisationId);

            if (organisation == null)
            {
                throw new ArgumentException(string.Format("Could not find an organisation with id {0}",
                    query.OrganisationId));
            }

            var organisationUsers =
                await context.OrganisationUsers.Where(ou => ou.OrganisationId == query.OrganisationId).ToListAsync();

            return organisationUsers.Select(item => organisationUserMap.Map(item)).ToList();
        }
    }
}
