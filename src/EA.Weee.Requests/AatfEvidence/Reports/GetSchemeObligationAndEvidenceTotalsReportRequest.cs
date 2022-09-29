namespace EA.Weee.Requests.AatfEvidence.Reports
{
    using System;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetSchemeObligationAndEvidenceTotalsReportRequest : IRequest<CSVFileData>
    {
        public Guid? SchemeId { get; private set; }

        public int ComplianceYear { get; private set; }

        public GetSchemeObligationAndEvidenceTotalsReportRequest(Guid? schemeId, int complianceYear) 
        {
            SchemeId = schemeId;
            ComplianceYear = complianceYear;
        }
    }
}
