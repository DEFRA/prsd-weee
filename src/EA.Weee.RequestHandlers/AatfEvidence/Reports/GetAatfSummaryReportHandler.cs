namespace EA.Weee.RequestHandlers.AatfEvidence.Reports
{
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Constants;
    using Core.Helpers;
    using Core.Shared;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core;
    using Requests.AatfEvidence.Reports;

    internal class GetAatfSummaryReportHandler : IRequestHandler<GetAatfSummaryReportRequest, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceStoredProcedures evidenceStoredProcedures;
        private readonly ICsvWriter<AatfEvidenceSummaryTotalsData> csvWriter;

        public GetAatfSummaryReportHandler(IWeeeAuthorization authorization,
            IEvidenceStoredProcedures evidenceStoredProcedures, 
            ICsvWriter<AatfEvidenceSummaryTotalsData> csvWriter)
        {
            this.authorization = authorization;
            this.evidenceStoredProcedures = evidenceStoredProcedures;
            this.csvWriter = csvWriter;
        }

        public async Task<CSVFileData> HandleAsync(GetAatfSummaryReportRequest request)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureAatfHasOrganisationAccess(request.AatfId);
            
            var summaryData = await evidenceStoredProcedures.GetAatfEvidenceSummaryTotals(request.AatfId, request.ComplianceYear);

            csvWriter.DefineColumn(EvidenceReportConstants.Category, x => x.CategoryName);
            csvWriter.DefineColumn(EvidenceReportConstants.ApprovedEvidence, x => x.ApprovedReceived.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.ApprovedReuse, x => x.ApprovedReused.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.SubmittedEvidence, x => x.SubmittedReceived.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.SubmittedReuse, x => x.SubmittedReused.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.DraftEvidence, x => x.DraftReceived.ToTonnageDisplay());
            csvWriter.DefineColumn(EvidenceReportConstants.DraftReuse, x => x.DraftReused.ToTonnageDisplay());

            var fileContent = csvWriter.Write(summaryData);

            var fileName = $"{request.ComplianceYear}_Summary report{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}.csv";

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}