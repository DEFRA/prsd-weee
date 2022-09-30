namespace EA.Weee.Requests.AatfEvidence.Reports
{
    using System;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetSchemeObligationAndEvidenceTotalsReportRequest : IRequest<CSVFileData>
    {
        public Guid? SchemeId { get; private set; }

        public Guid? AppropriateAuthorityId { get; private set; }

        public int ComplianceYear { get; private set; }

        public GetSchemeObligationAndEvidenceTotalsReportRequest(Guid? schemeId, Guid? appropriateAuthorityId, int complianceYear) 
        {
            SchemeId = schemeId;
            AppropriateAuthorityId = appropriateAuthorityId;
            ComplianceYear = complianceYear;
        }
    }
}
