namespace EA.Weee.RequestHandlers.Organisations
{
    using Core.Organisations;
    using DataAccess.DataAccess;
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Security;
    using System.Threading.Tasks;
    using Weee.Security;

    public class OrganisationByRegistrationNumberHandler : IRequestHandler<OrganisationByRegistrationNumberValue, OrganisationData>
    {
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly IMapper mapper;

        public OrganisationByRegistrationNumberHandler(IWeeeAuthorization authorization, IOrganisationDataAccess organisationDataAccess, IMapper mapper)
        {
            this.organisationDataAccess = organisationDataAccess;
            this.authorization = authorization;
            this.mapper = mapper;
        }

        public async Task<OrganisationData> HandleAsync(OrganisationByRegistrationNumberValue message)
        {
            authorization.EnsureCanAccessExternalArea();

            var organisation = await organisationDataAccess.GetByRegistrationNumber(message.RegistrationNumber);

            if (organisation == null)
            {
                return null;
            }

            var organisationData = mapper.Map<Organisation, OrganisationData>(organisation);

            return organisationData;
        }
    }
}
