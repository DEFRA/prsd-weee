namespace EA.Weee.RequestHandlers.AatfEvidence.Reports
{
    using Core.Admin;
    using Core.Constants;
    using Core.Shared;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;
    using Prsd.Core;
    using Requests.AatfEvidence.Reports;
    using System.Threading.Tasks;
    using Domain.Organisation;
    using Security;

    internal class GetTransferNotesReportHandler : IRequestHandler<GetTransferNoteReportRequest, CSVFileData>
    {
        private readonly IEvidenceStoredProcedures evidenceStoredProcedures;
        private readonly ICsvWriter<TransferNoteData> csvWriter;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IWeeeAuthorization weeeAuthorization;

        public GetTransferNotesReportHandler(IEvidenceStoredProcedures evidenceStoredProcedures, 
            ICsvWriter<TransferNoteData> csvWriter, 
            IGenericDataAccess genericDataAccess, IWeeeAuthorization weeeAuthorization)
        {
            this.evidenceStoredProcedures = evidenceStoredProcedures;
            this.csvWriter = csvWriter;
            this.genericDataAccess = genericDataAccess;
            this.weeeAuthorization = weeeAuthorization;
        }

        public async Task<CSVFileData> HandleAsync(GetTransferNoteReportRequest request)
        {
            if (!request.OrganisationId.HasValue)
            {
                weeeAuthorization.EnsureCanAccessInternalArea();
            }
            else
            {
                weeeAuthorization.EnsureCanAccessExternalArea();
                weeeAuthorization.EnsureOrganisationAccess(request.OrganisationId.Value);
            }

            var reportData = await evidenceStoredProcedures.GetTransferNoteReport(request.ComplianceYear, request.OrganisationId);

            var isPbs = false;
            if (request.OrganisationId.HasValue)
            {
                var organisation = await genericDataAccess.GetById<Organisation>(request.OrganisationId.Value);
                isPbs = organisation.IsBalancingScheme;
            }

            csvWriter.DefineColumn(EvidenceReportConstants.TransferReference, x => x.TransferReference);
            csvWriter.DefineColumn(EvidenceReportConstants.Status, x => x.TransferStatus);
            csvWriter.DefineColumn(EvidenceReportConstants.TransferApprovalDate, x => x.TransferApprovalDate);
            if (!isPbs)
            {
                csvWriter.DefineColumn(EvidenceReportConstants.TransferredByName, x => x.TransferredByName);
                csvWriter.DefineColumn(EvidenceReportConstants.TransferredByApprovalNumber, x => x.TransferredByApprovalNumber);
            }
            csvWriter.DefineColumn(EvidenceReportConstants.Recipient, x => x.RecipientName);
            csvWriter.DefineColumn(EvidenceReportConstants.RecipientApprovalNumber, x => x.RecipientApprovalNumber);
            csvWriter.DefineColumn(EvidenceReportConstants.EvidenceNoteReference, x => x.EvidenceNoteReference);
            csvWriter.DefineColumn(EvidenceReportConstants.EvidenceNoteApprovedDate, x => x.EvidenceNoteApprovalDate);
            csvWriter.DefineColumn(EvidenceReportConstants.AatfEvidenceIssuedByName, x => x.AatfIssuedByName);
            csvWriter.DefineColumn(EvidenceReportConstants.AatfEvidenceIssuedByApprovalNumber, x => x.AatfIssuedByApprovalNumber);
            csvWriter.DefineColumn(EvidenceReportConstants.Protocol, x => x.Protocol);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat1Transferred, x => x.Cat1Received);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat2Transferred, x => x.Cat2Received);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat3Transferred, x => x.Cat3Received);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat4Transferred, x => x.Cat4Received);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat5Transferred, x => x.Cat5Received);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat6Transferred, x => x.Cat6Received);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat7Transferred, x => x.Cat7Received);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat8Transferred, x => x.Cat8Received);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat9Transferred, x => x.Cat9Received);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat10Transferred, x => x.Cat10Received);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat11Transferred, x => x.Cat11Received);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat12Transferred, x => x.Cat12Received);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat13Transferred, x => x.Cat13Received);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat14Transferred, x => x.Cat14Received);
            csvWriter.DefineColumn(EvidenceReportConstants.TotalTransferred, x => x.TotalReceived);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat1ReusedTransferred, x => x.Cat1Reused);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat2ReusedTransferred, x => x.Cat2Reused);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat3ReusedTransferred, x => x.Cat3Reused);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat4ReusedTransferred, x => x.Cat4Reused);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat5ReusedTransferred, x => x.Cat5Reused);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat6ReusedTransferred, x => x.Cat6Reused);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat7ReusedTransferred, x => x.Cat7Reused);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat8ReusedTransferred, x => x.Cat8Reused);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat9ReusedTransferred, x => x.Cat9Reused);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat10ReusedTransferred, x => x.Cat10Reused);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat11ReusedTransferred, x => x.Cat11Reused);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat12ReusedTransferred, x => x.Cat12Reused);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat13ReusedTransferred, x => x.Cat13Reused);
            csvWriter.DefineColumn(EvidenceReportConstants.Cat14ReusedTransferred, x => x.Cat14Reused);
            csvWriter.DefineColumn(EvidenceReportConstants.TotalReusedTransferred, x => x.TotalReused);

            var fileContent = csvWriter.Write(reportData);
            var timestamp = SystemTime.Now;

            var fileName = $"{request.ComplianceYear}_Transfer notes report{timestamp.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}.csv";
            
            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}