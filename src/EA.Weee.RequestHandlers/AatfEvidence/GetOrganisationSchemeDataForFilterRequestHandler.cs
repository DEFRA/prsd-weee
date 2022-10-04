namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess.DataAccess;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Security;

    internal class GetOrganisationSchemeDataForFilterRequestHandler : IRequestHandler<GetOrganisationSchemeDataForFilterRequest, List<EntityIdDisplayNameData>>
    {
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IWeeeAuthorization weeeAuthorization;

        public GetOrganisationSchemeDataForFilterRequestHandler(IWeeeAuthorization weeeAuthorization, IEvidenceDataAccess evidenceDataAccess)
        {
            this.evidenceDataAccess = evidenceDataAccess;
            this.weeeAuthorization = weeeAuthorization;
        }

        public async Task<List<EntityIdDisplayNameData>> HandleAsync(GetOrganisationSchemeDataForFilterRequest request)
        {
            if (request.AatfId.HasValue)
            {
                weeeAuthorization.EnsureCanAccessExternalArea();
                weeeAuthorization.EnsureAatfHasOrganisationAccess(request.AatfId.Value);
            }
            else
            {
                weeeAuthorization.EnsureCanAccessInternalArea();
            }

            var organisations =
                await evidenceDataAccess.GetRecipientOrganisations(request.AatfId, request.ComplianceYear);

            return organisations.Select(x =>
                    new EntityIdDisplayNameData() { DisplayName = x.IsBalancingScheme ? x.OrganisationName : x.Scheme.SchemeName, Id = x.Id })
                .OrderBy(x => x.DisplayName)
                .ToList();
        }
    }
}