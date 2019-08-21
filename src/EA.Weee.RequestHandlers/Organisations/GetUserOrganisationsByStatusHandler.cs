namespace EA.Weee.RequestHandlers.Organisations
{
    using Core.Organisations;
    using DataAccess;
    using Domain.Organisation;
    using Prsd.Core.Domain;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    internal class GetUserOrganisationsByStatusHandler :
        IRequestHandler<GetUserOrganisationsByStatus, List<OrganisationUserData>>
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;
        private readonly IMap<OrganisationUser, OrganisationUserData> organisationUserMap;

        public GetUserOrganisationsByStatusHandler(WeeeContext context, IUserContext userContext,
            IMap<OrganisationUser, OrganisationUserData> organisationUserMap)
        {
            this.context = context;
            this.userContext = userContext;
            this.organisationUserMap = organisationUserMap;
        }

        public async Task<List<OrganisationUserData>> HandleAsync(GetUserOrganisationsByStatus query)
        {
            var organisationUsers = new List<OrganisationUser>();
            string userId = userContext.UserId.ToString();

            if (query.OrganisationUserStatus.Length > 0)
            {
                organisationUsers =
                    await
                        context.OrganisationUsers.Where(
                            ou =>
                                query.OrganisationUserStatus.Contains(ou.UserStatus.Value) && ou.UserId == userId)
                            .ToListAsync();
            }
            else
            {
                organisationUsers =
                    await
                        context.OrganisationUsers.Where(
                            ou => ou.UserId == userId)
                            .ToListAsync();
            }

            if (query.OrganisationStatus != null &&
                query.OrganisationStatus.Length > 0)
            {
                organisationUsers = organisationUsers.Where(ou => query.OrganisationStatus.Contains(ou.Organisation.OrganisationStatus.Value)).ToList();
            }

            var organisationUserData = organisationUsers.Select(item => organisationUserMap.Map(item)).ToList();

            return organisationUserData;
        }
    }
}
