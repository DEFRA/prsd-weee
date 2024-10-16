﻿namespace EA.Weee.RequestHandlers.Shared
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin.Obligation;
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
            if (!message.OrganisationId.HasValue)
            {
                authorization.EnsureCanAccessInternalArea();
            }
            else
            {
                authorization.EnsureCanAccessExternalArea();
                authorization.EnsureOrganisationAccess(message.OrganisationId.Value);
            }

            var summaryData = await evidenceStoredProcedures.GetObligationEvidenceSummaryTotals(message.SchemeId, message.OrganisationId, message.ComplianceYear);

            var result = mapper.Map<List<ObligationEvidenceSummaryTotalsData>, ObligationEvidenceSummaryData>(summaryData);

            return result;
        }
    }
}
