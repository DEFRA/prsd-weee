namespace EA.Weee.Requests.AatfEvidence.Reports
{
    using System;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetTransferNoteReportRequest : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }

        public Guid? OrganisationId { get; private set; }

        public GetTransferNoteReportRequest(int complianceYear, Guid? organisationId)
        {
            OrganisationId = organisationId;
            ComplianceYear = complianceYear;
        }
    }
}
