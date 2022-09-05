namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Scheme;
    using EA.Weee.DataAccess.DataAccess;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;

    internal class GetOrganisationSchemeDataForFilterRequestHandler : IRequestHandler<GetOrganisationSchemeDataForFilterRequest, List<OrganisationSchemeData>>
    {
        private readonly IEvidenceDataAccess evidenceDataAccess;

        public GetOrganisationSchemeDataForFilterRequestHandler(IEvidenceDataAccess evidenceDataAccess)
        {
            this.evidenceDataAccess = evidenceDataAccess;
        }

        public async Task<List<OrganisationSchemeData>> HandleAsync(GetOrganisationSchemeDataForFilterRequest message)
        {
            var organisations =
                await evidenceDataAccess.GetOrganisationsWithNotes(message.OrganisationId, message.ComplianceYear);

            return organisations.Select(x =>
                    new OrganisationSchemeData() { DisplayName = x.IsBalancingScheme ? x.OrganisationName : x.Scheme.SchemeName, Id = x.Id })
                .OrderBy(x => x.DisplayName)
                .ToList();
        }
    }
}