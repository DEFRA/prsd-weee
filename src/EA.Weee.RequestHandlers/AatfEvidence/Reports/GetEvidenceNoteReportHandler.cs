namespace EA.Weee.RequestHandlers.AatfEvidence.Reports
{
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Constants;
    using Core.Helpers;
    using Core.Shared;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core;
    using Requests.AatfEvidence.Reports;

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

        public async Task<CSVFileData> HandleAsync(GetEvidenceNoteReportRequest request)
        {
            if (!request.OriginatorOrganisationId.HasValue && !request.RecipientOrganisationId.HasValue)
            {
                authorization.EnsureCanAccessInternalArea();
            }

            if (request.OriginatorOrganisationId.HasValue)
            {
                authorization.EnsureOrganisationAccess(request.OriginatorOrganisationId.Value);
            }

            if (request.RecipientOrganisationId.HasValue)
            {
                authorization.EnsureOrganisationAccess(request.RecipientOrganisationId.Value);
            }
            
            var reportData = await evidenceStoredProcedures.GetEvidenceNoteOriginalTonnagesReport(
                request.ComplianceYear,
                request.OriginatorOrganisationId,
                request.RecipientOrganisationId);

            csvWriter.DefineColumn(EvidenceReportConstants.Reference, x => x.Reference);
            csvWriter.DefineColumn(EvidenceReportConstants.Status, x => x.Status);
            csvWriter.DefineColumn(EvidenceReportConstants.AppropriateAuthority, x => x.AppropriateAuthority);
            csvWriter.DefineColumn(EvidenceReportConstants.SubmittedDate, x => x.SubmittedDateTime);
            csvWriter.DefineColumn(EvidenceReportConstants.SubmittedAatf, x => x.SubmittedByAatf);
            csvWriter.DefineColumn(EvidenceReportConstants.SubmittedAatfApprovalNumber, x => x.AatfApprovalNumber);
            csvWriter.DefineColumn(EvidenceReportConstants.ObligationType, x => x.ObligationType);
            csvWriter.DefineColumn(EvidenceReportConstants.ReceivedStartDate, x => x.ReceivedStartDate);
            csvWriter.DefineColumn(EvidenceReportConstants.ReceivedEndDate, x => x.ReceivedEndDate);
            csvWriter.DefineColumn(EvidenceReportConstants.Recipient, x => x.Recipient);
            csvWriter.DefineColumn(EvidenceReportConstants.RecipientApprovalNumber, x => x.RecipientApprovalNumber);
            csvWriter.DefineColumn(EvidenceReportConstants.Protocol, x => x.Protocol);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat1Received, x => x.Cat1Received.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat2Received, x => x.Cat2Received.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat3Received, x => x.Cat3Received.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat4Received, x => x.Cat4Received.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat5Received, x => x.Cat5Received.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat6Received, x => x.Cat6Received.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat7Received, x => x.Cat7Received.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat8Received, x => x.Cat8Received.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat9Received, x => x.Cat9Received.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat10Received, x => x.Cat10Received.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat11Received, x => x.Cat11Received.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat12Received, x => x.Cat12Received.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat13Received, x => x.Cat13Received.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat14Received, x => x.Cat14Received.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.TotalReceived, x => x.TotalReceived.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat1Reused, x => x.Cat1Reused.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat2Reused, x => x.Cat2Reused.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat3Reused, x => x.Cat3Reused.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat4Reused, x => x.Cat4Reused.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat5Reused, x => x.Cat5Reused.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat6Reused, x => x.Cat6Reused.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat7Reused, x => x.Cat7Reused.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat8Reused, x => x.Cat8Reused.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat9Reused, x => x.Cat9Reused.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat10Reused, x => x.Cat10Reused.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat11Reused, x => x.Cat11Reused.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat12Reused, x => x.Cat12Reused.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat13Reused, x => x.Cat13Reused.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.Cat14Reused, x => x.Cat14Reused.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.TotalReused, x => x.TotalReused.ToTonnageDisplay());

            var fileContent = csvWriter.Write(reportData);
            var timestamp = SystemTime.Now;
            var fileName = $"{request.ComplianceYear}_Evidence notes original tonnages_{timestamp.ToString(DateTimeConstants.FilenameTimestampFormat)}.csv";

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}