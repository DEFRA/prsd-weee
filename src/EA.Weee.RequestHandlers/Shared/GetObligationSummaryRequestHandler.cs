namespace EA.Weee.RequestHandlers.Shared
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin.Obligation;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.Requests.Shared;
    using Security;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class GetObligationSummaryRequestHandler : IRequestHandler<GetObligationSummaryRequest, ObligationEvidenceSummaryData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IMapper mapper;
        private readonly IEvidenceStoredProcedures evidenceStoredProcedures;
        private readonly IOrganisationDataAccess organisationDataAccess;

        public GetObligationSummaryRequestHandler(IWeeeAuthorization authorization,
            IMapper mapper,
            IEvidenceStoredProcedures evidenceStoredProcedures,
            IOrganisationDataAccess organisationDataAccess)
        {
            this.authorization = authorization;
            this.mapper = mapper;
            this.evidenceStoredProcedures = evidenceStoredProcedures;
            this.organisationDataAccess = organisationDataAccess;
        }

        public async Task<ObligationEvidenceSummaryData> HandleAsync(GetObligationSummaryRequest message)
        {
            if (message.InternalAccess)
            {
                authorization.EnsureCanAccessInternalArea();
            }
            else
            {
                authorization.EnsureCanAccessExternalArea();
                authorization.EnsureOrganisationAccess(message.OrganisationId);
            }

            var summaryData = await evidenceStoredProcedures.GetObligationEvidenceSummaryTotals(message.SchemeId, message.ComplianceYear);

            var result = mapper.Map<List<ObligationEvidenceSummaryTotalsData>, ObligationEvidenceSummaryData>(summaryData);

            return result;
        }
    }
}
