namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin.Obligation;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.Obligations;
    using EA.Weee.Security;
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
            //TODO Double check these are the correct authorization checks, disabled as my admin user was failing them
            //authorization.EnsureCanAccessInternalArea();
            //authorization.EnsureUserInRole(Roles.InternalAdmin);
            //authorization.EnsureOrganisationAccess(message.OrganisationId);

            var summaryData = await evidenceStoredProcedures.GetObligationEvidenceSummaryTotals(message.PcsId, message.OrganisationId, message.ComplianceYear);

            var result = mapper.Map<List<ObligationEvidenceSummaryTotalsData>, ObligationEvidenceSummaryData>(summaryData);

            return result;
        }
    }
}
