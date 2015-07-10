namespace EA.Weee.RequestHandlers.Organisations
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations;

    internal class GetOrganisationsByUserIdHandler :
        IRequestHandler<GetOrganisationsByUserId, List<OrganisationUserData>>
    {
        private readonly WeeeContext context;
        private readonly IMap<OrganisationUser, OrganisationUserData> organisationUserMap;

        public GetOrganisationsByUserIdHandler(WeeeContext context,
            IMap<OrganisationUser, OrganisationUserData> organisationUserMap)
        {
            this.context = context;
            this.organisationUserMap = organisationUserMap;
        }

        public async Task<List<OrganisationUserData>> HandleAsync(GetOrganisationsByUserId query)
        {
            var organisationUsers = new List<OrganisationUser>();

            if (query.OrganisationUserStatus.Length > 0)
            {
                organisationUsers =
                    await
                        context.OrganisationUsers.Where(
                            ou =>
                                query.OrganisationUserStatus.Contains(ou.UserStatus.Value) && ou.UserId == query.UserId)
                            .ToListAsync();
            }
            else
            {
                organisationUsers =
                    await
                        context.OrganisationUsers.Where(
                            ou => ou.UserId == query.UserId)
                            .ToListAsync();
            }

            var organisationUserData = organisationUsers.Select(item => organisationUserMap.Map(item)).ToList();

            return organisationUserData;
        }
    }
}
