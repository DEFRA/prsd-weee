namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Prsd.Core.Domain;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations;

    internal class GetApprovedOrganisationsByUserIdHandler : IRequestHandler<GetApprovedOrganisationsByUserId, List<OrganisationUserData>>
    {
        private readonly WeeeContext context;
        private IMap<OrganisationUser, OrganisationUserData> organisationUserMap;

        public GetApprovedOrganisationsByUserIdHandler(WeeeContext context, IUserContext userContext, IMap<OrganisationUser, OrganisationUserData> organisationUserMap)
        {
            this.context = context;
            this.organisationUserMap = organisationUserMap;
        }

        public async Task<List<OrganisationUserData>> HandleAsync(GetApprovedOrganisationsByUserId query)
        {
            var organisationUsers =
                await
                    context.OrganisationUsers.Where(
                        ou =>
                            ou.Organisation.OrganisationStatus.Value == OrganisationStatus.Approved.Value && ou.UserId == query.UserId).ToListAsync();

            var organisationUserData = organisationUsers.Select(item => organisationUserMap.Map(item)).ToList();

            return organisationUserData;
        }
    }
}
