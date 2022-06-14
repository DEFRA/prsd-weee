namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AatfReturn.Internal;
    using DataAccess.StoredProcedure;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.DataAccess.DataAccess;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Security;
    using NoteStatus = Domain.Evidence.NoteStatus;

    internal class GetAatfSummaryRequestHandler : IRequestHandler<GetAatfSummaryRequest, AatfEvidenceSummaryData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;
        private readonly IEvidenceStoredProcedures evidenceStoredProcedures;

        public GetAatfSummaryRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess evidenceDataAccess,
            IMapper mapper, 
            IEvidenceStoredProcedures evidenceStoredProcedures)
        {
            this.authorization = authorization;
            this.evidenceDataAccess = evidenceDataAccess;
            this.mapper = mapper;
            this.evidenceStoredProcedures = evidenceStoredProcedures;
        }

        public async Task<AatfEvidenceSummaryData> HandleAsync(GetAatfSummaryRequest message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureAatfHasOrganisationAccess(message.AatfId);
            
            var summaryData = await evidenceStoredProcedures.GetAatfEvidenceSummaryTotals(message.AatfId, (short)message.ComplianceYear);

            var approvedNotes = await evidenceDataAccess.GetNoteCountByStatusAndAatf(NoteStatus.Approved, message.AatfId);
            var submittedNotes = await evidenceDataAccess.GetNoteCountByStatusAndAatf(NoteStatus.Submitted, message.AatfId);
            var draftNotes = await evidenceDataAccess.GetNoteCountByStatusAndAatf(NoteStatus.Draft, message.AatfId);

            var result = new AatfEvidenceSummaryData(
                mapper.Map<List<AatfEvidenceSummaryTotalsData>, List<EvidenceSummaryTonnageData>>(summaryData),
                draftNotes,
                submittedNotes,
                approvedNotes);

            return result;
        }
    }
}