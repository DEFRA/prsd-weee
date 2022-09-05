namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Scheme;
    using EA.Weee.DataAccess.DataAccess;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Security;

    internal class GetOrganisationSchemeDataForFilterRequestHandler : IRequestHandler<GetOrganisationSchemeDataForFilterRequest, List<OrganisationSchemeData>>
    {
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IWeeeAuthorization weeeAuthorization;

        public GetOrganisationSchemeDataForFilterRequestHandler(IWeeeAuthorization weeeAuthorization, IEvidenceDataAccess evidenceDataAccess)
        {
            this.evidenceDataAccess = evidenceDataAccess;
            this.weeeAuthorization = weeeAuthorization;
        }

        public async Task<List<OrganisationSchemeData>> HandleAsync(GetOrganisationSchemeDataForFilterRequest request)
        {
            weeeAuthorization.EnsureCanAccessExternalArea();
            weeeAuthorization.EnsureOrganisationAccess(request.OrganisationId);

            var organisations =
                await evidenceDataAccess.GetRecipientOrganisations(request.OrganisationId, request.ComplianceYear);

            return organisations.Select(x =>
                    new OrganisationSchemeData() { DisplayName = x.IsBalancingScheme ? x.OrganisationName : x.Scheme.SchemeName, Id = x.Id })
                .OrderBy(x => x.DisplayName)
                .ToList();
        }
    }
}