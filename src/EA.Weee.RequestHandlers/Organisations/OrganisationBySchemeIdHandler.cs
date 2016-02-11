namespace EA.Weee.RequestHandlers.Organisations
{
    using System.Threading.Tasks;
    using Core.Organisations;
    using Core.Security;
    using DataAccess.DataAccess;
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Security;

    public class OrganisationBySchemeIdHandler : IRequestHandler<OrganisationBySchemeId, OrganisationData>
    {
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly IMapper mapper;

        public OrganisationBySchemeIdHandler(IWeeeAuthorization authorization, IOrganisationDataAccess organisationDataAccess, IMapper mapper)
        {
            this.organisationDataAccess = organisationDataAccess;
            this.authorization = authorization;
            this.mapper = mapper;
        }

        public async Task<OrganisationData> HandleAsync(OrganisationBySchemeId message)
        {
            authorization.CheckInternalOrSchemeAccess(message.SchemeId);

            var organisation = await organisationDataAccess.GetBySchemeId(message.SchemeId);
            var organisationData = mapper.Map<Organisation, OrganisationData>(organisation);

            organisationData.CanEditOrganisation = authorization.CheckUserInRole(Roles.InternalAdmin);

            return organisationData;
        }
    }
}
