namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin.Obligation;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.Requests.Admin.Obligations;
    using Security;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class GetObligationSummaryRequestHandler : IRequestHandler<GetObligationSummaryRequest, ObligationEvidenceSummaryData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IMapper mapper;
        private readonly IEvidenceStoredProcedures evidenceStoredProcedures;

        public GetObligationSummaryRequestHandler(IWeeeAuthorization authorization,
            IMapper mapper,
            IEvidenceStoredProcedures evidenceStoredProcedures)
        {
            this.authorization = authorization;
            this.mapper = mapper;
            this.evidenceStoredProcedures = evidenceStoredProcedures;
        }

        public async Task<ObligationEvidenceSummaryData> HandleAsync(GetObligationSummaryRequest message)
        {
            authorization.EnsureCanAccessInternalArea();

            var summaryData = await evidenceStoredProcedures.GetObligationEvidenceSummaryTotals(message.SchemeId, message.ComplianceYear);

            var result = mapper.Map<List<ObligationEvidenceSummaryTotalsData>, ObligationEvidenceSummaryData>(summaryData);

            return result;
        }
    }
}
