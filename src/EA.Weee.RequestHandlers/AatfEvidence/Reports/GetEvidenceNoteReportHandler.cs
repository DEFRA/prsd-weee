namespace EA.Weee.RequestHandlers.AatfEvidence.Reports
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfEvidence;
    using Requests.AatfEvidence.Reports;
    using NoteStatus = Domain.Evidence.NoteStatus;

    internal class GetEvidenceNoteReportHandler : IRequestHandler<GetEvidenceNoteReportRequest, CSVFileData>
    {
        private readonly IEvidenceStoredProcedures evidenceStoredProcedures;
        private readonly ICsvWriter<EvidenceNoteReportData> csvWriter;
        private readonly IWeeeAuthorization authorization;

        public GetEvidenceNoteReportHandler(IWeeeAuthorization authorization,
            IEvidenceStoredProcedures evidenceStoredProcedures, ICsvWriter<EvidenceNoteReportData> csvWriter)
        {
            this.authorization = authorization;
            this.evidenceStoredProcedures = evidenceStoredProcedures;
            this.csvWriter = csvWriter;
        }

        public async Task<CSVFileData> HandleAsync(GetEvidenceNoteReportRequest message)
        {
            if (!message.OriginatorOrganisationId.HasValue && !message.RecipientOrganisationId.HasValue)
            {
                authorization.CheckCanAccessInternalArea();
            }

            if (message.OriginatorOrganisationId.HasValue)
            {
                authorization.CheckInternalOrOrganisationAccess(message.OriginatorOrganisationId.Value);
            }

            if (message.RecipientOrganisationId.HasValue)
            {
                authorization.CheckInternalOrOrganisationAccess(message.RecipientOrganisationId.Value);
            }


            var summaryData = await evidenceStoredProcedures.GetAatfEvidenceSummaryTotals(message.AatfId, message.ComplianceYear);

            

            return result;
        }
    }
}