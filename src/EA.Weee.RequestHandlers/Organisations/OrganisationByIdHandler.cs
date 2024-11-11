namespace EA.Weee.RequestHandlers.Organisations
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    internal class OrganisationByIdHandler : IRequestHandler<GetOrganisationInfo, OrganisationData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly IMap<Organisation, OrganisationData> organisationMap;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public OrganisationByIdHandler(
            IWeeeAuthorization authorization,
            WeeeContext context,
            IMap<Organisation, OrganisationData> organisationMap,
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

            var org = await context.Organisations
                .SingleOrDefaultAsync(o => o.Id == query.OrganisationId);

            if (org == null)
            {
                throw new ArgumentException($"Could not find an organisation with id {query.OrganisationId}");
            }

            var organisationData = organisationMap.Map(org);

            await PopulateAdditionalData(organisationData, query.OrganisationId);

            return organisationData;
        }

        private async Task PopulateAdditionalData(OrganisationData organisationData, Guid organisationId)
        {
            await SetSchemeId(organisationData, organisationId);
            await SetFacilityInfo(organisationData, organisationId);
            await SetIsRepresentingCompany(organisationData, organisationId);
            await SetDirectRegistrants(organisationData, organisationId);
        }

        private async Task SetSchemeId(OrganisationData organisationData, Guid organisationId)
        {
            var schemes = await context.Schemes
                .SingleOrDefaultAsync(o => o.OrganisationId == organisationId);

            if (schemes != null)
            {
                organisationData.SchemeId = schemes.Id;
            }
        }

        private async Task SetFacilityInfo(OrganisationData organisationData, Guid organisationId)
        {
            var facilities = await context.Aatfs
                .Where(o => o.Organisation.Id == organisationId)
                .Select(o => o.FacilityType.Value)
                .Distinct()
                .ToListAsync();

            organisationData.HasAatfs = facilities.Contains((int)FacilityType.Aatf.Value);
            organisationData.HasAes = facilities.Contains((int)FacilityType.Ae.Value);
        }

        private async Task SetIsRepresentingCompany(OrganisationData organisationData, Guid organisationId)
        {
            organisationData.IsRepresentingCompany = await context.DirectRegistrants
                .AnyAsync(d => d.AuthorisedRepresentativeId.HasValue && d.OrganisationId == organisationId);
        }

        private async Task SetDirectRegistrants(OrganisationData organisationData, Guid organisationId)
        {
            if (organisationData.DirectRegistrants == null)
            {
                organisationData.DirectRegistrants = new List<DirectRegistrantInfo>();
            }

            var systemTime = await systemDataDataAccess.GetSystemDateTime();
            var currentYear = systemTime.Year;

            var validStatuses = new[]
            {
                DirectProducerSubmissionStatus.Complete.Value
            };

            // Ensure proper materialization of the query with explicit ordering
            var directRegistrants = await context.DirectRegistrants
                .Where(o => o.OrganisationId == organisationId)
                .Select(dr => new
                {
                    dr.Id,
                    dr.AuthorisedRepresentativeId,
                    OverseasProducerName = dr.AuthorisedRepresentative.OverseasProducerName,
                    HasCurrentYearSubmission = dr.DirectProducerSubmissions
                        .Any(submission => submission.ComplianceYear == currentYear),
                    MostRecentSubmittedYear = dr.DirectProducerSubmissions
                        .Where(submission => (currentYear > submission.ComplianceYear) ||
                                             validStatuses.Contains(submission.DirectProducerSubmissionStatus.Value))
                        .Select(submission => submission.ComplianceYear)
                        .OrderByDescending(x => x)
                        .FirstOrDefault()
                })
                .ToListAsync();

            foreach (var directRegistrant in directRegistrants)
            {
                organisationData.DirectRegistrants.Add(new DirectRegistrantInfo
                {
                    DirectRegistrantId = directRegistrant.Id,
                    YearSubmissionStarted = directRegistrant.HasCurrentYearSubmission,
                    RepresentedCompanyName = directRegistrant.AuthorisedRepresentativeId.HasValue
                        ? directRegistrant.OverseasProducerName
                        : null,
                    MostRecentSubmittedYear = directRegistrant.MostRecentSubmittedYear
                });
            }
        }
    }
}