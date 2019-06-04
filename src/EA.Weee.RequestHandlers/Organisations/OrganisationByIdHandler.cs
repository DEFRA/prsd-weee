﻿namespace EA.Weee.RequestHandlers.Organisations
{
    using EA.Weee.Security;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Core.Organisations;
    using DataAccess;
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Security;

    internal class OrganisationByIdHandler : IRequestHandler<GetOrganisationInfo, OrganisationData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly IMap<Organisation, OrganisationData> organisationMap;

        public OrganisationByIdHandler(IWeeeAuthorization authorization, WeeeContext context, IMap<Organisation, OrganisationData> organisationMap)
        {
            this.authorization = authorization;
            this.context = context;
            this.organisationMap = organisationMap;
        }

        public async Task<OrganisationData> HandleAsync(GetOrganisationInfo query)
        {
            authorization.EnsureInternalOrOrganisationAccess(query.OrganisationId);

            // Need to materialize EF request before mapping (because mapping parses enums)
            var org = await context.Organisations
                .SingleOrDefaultAsync(o => o.Id == query.OrganisationId);

            if (org == null)
            {
                throw new ArgumentException($"Could not find an organisation with id {query.OrganisationId}");
            }

            var organisationData = organisationMap.Map(org);

            var schemes = await context.Schemes.SingleOrDefaultAsync(o => o.OrganisationId == query.OrganisationId);

            if (schemes != null)
            {
                organisationData.SchemeId = schemes.Id;
            }

            organisationData.HasAatfs = await context.Aatfs.CountAsync(o => o.Organisation.Id == query.OrganisationId) > 0;

            return organisationData;
        }
    }
}