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
    using Domain.Scheme;
    using Security;

    internal class GetSchemeObligationAndEvidenceTotalsReportHandler : IRequestHandler<GetSchemeObligationAndEvidenceTotalsReportRequest, CSVFileData>
    {
        private readonly IEvidenceStoredProcedures evidenceStoredProcedures;
        private readonly ICsvWriter<ObligationAndEvidenceProgressSummaryData> csvWriter;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IWeeeAuthorization weeeAuthorization;

        public GetSchemeObligationAndEvidenceTotalsReportHandler(IEvidenceStoredProcedures evidenceStoredProcedures, 
            ICsvWriter<ObligationAndEvidenceProgressSummaryData> csvWriter, 
            IGenericDataAccess genericDataAccess, IWeeeAuthorization weeeAuthorization)
        {
            this.evidenceStoredProcedures = evidenceStoredProcedures;
            this.csvWriter = csvWriter;
            this.genericDataAccess = genericDataAccess;
            this.weeeAuthorization = weeeAuthorization;
        }
         
        public async Task<CSVFileData> HandleAsync(GetSchemeObligationAndEvidenceTotalsReportRequest request)
        {
            if (request.OrganisationId.HasValue)
            {
                weeeAuthorization.EnsureCanAccessExternalArea();
                weeeAuthorization.EnsureOrganisationAccess(request.OrganisationId.Value);
            }
            else
            {
                weeeAuthorization.EnsureCanAccessInternalArea();
            }
            
            var reportData = await evidenceStoredProcedures.GetSchemeObligationAndEvidenceProgress(request.SchemeId, request.AppropriateAuthorityId,  request.OrganisationId, request.ComplianceYear);

            if (!request.OrganisationId.HasValue)
            {
                csvWriter.DefineColumn(EvidenceReportConstants.SchemeName, x => x.SchemeName);
                csvWriter.DefineColumn(EvidenceReportConstants.SchemeApprovalNumber, x => x.ApprovalNumber);
            }

            csvWriter.DefineColumn(EvidenceReportConstants.Category, x => x.CategoryName);
            csvWriter.DefineColumn(EvidenceReportConstants.HouseholdObligation, x => x.Obligation);
            csvWriter.DefineColumn(EvidenceReportConstants.HouseholdEvidence, x => x.Evidence);
            csvWriter.DefineColumn(EvidenceReportConstants.HouseholdReuse, x => x.Reuse);
            csvWriter.DefineColumn(EvidenceReportConstants.TransferredOut, x => x.TransferredOut);
            csvWriter.DefineColumn(EvidenceReportConstants.TransferredIn, x => x.TransferredIn);
            csvWriter.DefineColumn(EvidenceReportConstants.Difference, x => x.ObligationDifference);
            csvWriter.DefineColumn(EvidenceReportConstants.NonHouseholdEvidence, x => x.NonHouseholdEvidence);
            csvWriter.DefineColumn(EvidenceReportConstants.NonHouseholdReuse, x => x.NonHouseHoldEvidenceReuse);

            var fileContent = csvWriter.Write(reportData);
            var timestamp = SystemTime.Now;

            var approvalNumber = string.Empty;
            if (request.SchemeId.HasValue)
            {
                var scheme = await genericDataAccess.GetById<Scheme>(request.SchemeId.Value);
                approvalNumber = $"_{scheme.ApprovalNumber}";
            }
            
            var fileName = $"{request.ComplianceYear}{approvalNumber}_PCS evidence and obligation progress{timestamp.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}.csv";
            if (request.OrganisationId.HasValue)
            {
                fileName = $"{request.ComplianceYear}_PCS Summary{timestamp.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}.csv";
            }

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}