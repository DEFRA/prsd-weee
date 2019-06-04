namespace EA.Weee.RequestHandlers.Admin
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Security;

    internal class OrganisationInternalByIdHandler : IRequestHandler<GetInternalOrganisation, OrganisationData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly IMap<Organisation, OrganisationData> organisationMap;

        public OrganisationInternalByIdHandler(IWeeeAuthorization authorization, WeeeContext context, IMap<Organisation, OrganisationData> organisationMap)
        {
            this.authorization = authorization;
            this.context = context;
            this.organisationMap = organisationMap;
        }

        public async Task<OrganisationData> HandleAsync(GetInternalOrganisation query)
        {
            authorization.EnsureCanAccessInternalArea();

            var org = await context.Organisations.SingleOrDefaultAsync(o => o.Id == query.OrganisationId);

            if (org == null)
            {
                throw new ArgumentException($"Could not find an organisation with id {query.OrganisationId}");
            }

            var organisationData = organisationMap.Map(org);

            organisationData.CanEditOrganisation = authorization.CheckUserInRole(Roles.InternalAdmin);

            return organisationData;
        }
    }
}