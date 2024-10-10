namespace EA.Weee.RequestHandlers.Organisations
{
    using Core.Organisations;
    using DataAccess;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using EA.Prsd.Core;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Producer;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Security;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    internal class OrganisationByIdHandler : IRequestHandler<GetOrganisationInfo, OrganisationData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly IMap<Organisation, OrganisationData> organisationMap;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public OrganisationByIdHandler(IWeeeAuthorization authorization,
            WeeeContext context,
            IMap<Organisation, 
            OrganisationData> organisationMap,
            ISystemDataDataAccess systemDataDataAccess)
        {
            this.authorization = authorization;
            this.context = context;
            this.organisationMap = organisationMap;
            this.systemDataDataAccess = systemDataDataAccess;
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

            organisationData.HasAatfs = await context.Aatfs.AnyAsync(o => o.Organisation.Id == query.OrganisationId && o.FacilityType.Value == (int)FacilityType.Aatf.Value);
            organisationData.HasAes = await context.Aatfs.AnyAsync(o => o.Organisation.Id == query.OrganisationId && o.FacilityType.Value == (int)FacilityType.Ae.Value);

            var directRegistrants = await context.DirectRegistrants.Where(o => o.OrganisationId == query.OrganisationId).ToListAsync();

            var systemTime = await systemDataDataAccess.GetSystemDateTime();
            int currentYear = systemTime.Year;

            foreach (var directRegistrant in directRegistrants)
            {
                var yearSubmissionStarted = directRegistrant.DirectProducerSubmissions
                    .Any(submission => submission.ComplianceYear == currentYear);

                organisationData.DirectRegistrants.Add(new DirectRegistrantInfo
                {
                    DirectRegistrantId = directRegistrant.Id,
                    YearSubmissionStarted = yearSubmissionStarted
                });
            }

            return organisationData;
        }
    }
}