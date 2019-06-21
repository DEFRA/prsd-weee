namespace EA.Weee.RequestHandlers.Organisations.GetActiveOrganisationUsers
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Organisations.GetActiveOrganisationUsers.DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class GetActiveOrganisationUsersHandler : IRequestHandler<GetActiveOrganisationUsers, IEnumerable<OrganisationUserData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetActiveOrganisationUsersDataAccess dataAccess;
        private readonly IMap<OrganisationUser, OrganisationUserData> mapper;

        public GetActiveOrganisationUsersHandler(IWeeeAuthorization authorization, IGetActiveOrganisationUsersDataAccess dataAccess, IMap<OrganisationUser, OrganisationUserData> mapper)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<OrganisationUserData>> HandleAsync(GetActiveOrganisationUsers message)
        {
            var result = new List<OrganisationUserData>();
            var users = await dataAccess.FetchActiveOrganisationUsers(message.OrganisationId);

            foreach (var user in users)
            {
                result.Add(mapper.Map(user));
            }

            return result;
        }
    }
}
