namespace EA.Weee.RequestHandlers.Organisations
{
    using Core.Organisations;
    using DataAccess.DataAccess;
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Security;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class OrganisationByRegistrationNumberHandler : IRequestHandler<OrganisationByRegistrationNumberValue, List<OrganisationData>>
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

        public async Task<List<OrganisationData>> HandleAsync(OrganisationByRegistrationNumberValue message)
        {
            authorization.EnsureCanAccessExternalArea();

            var organisations = await organisationDataAccess.GetByRegistrationNumber(message.RegistrationNumber);

            return organisations.Count == 0 ? new List<OrganisationData>() : organisations.Select(organisation => mapper.Map<Organisation, OrganisationData>(organisation)).ToList();
        }
    }
}
