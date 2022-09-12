namespace EA.Weee.Requests.AatfEvidence.Reports
{
    using System;
    using Core.AatfEvidence;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetEvidenceReportBaseRequest : IRequest<CSVFileData>
    {
        public Guid? RecipientOrganisationId { get; protected set; }

        public Guid? OriginatorOrganisationId { get; protected set; }

        public Guid? AatfId { get; protected set; }

        public int ComplianceYear { get; protected set; }

        public GetEvidenceReportBaseRequest(Guid? recipientOrganisationId, 
            Guid? originatorOrganisationId,
            Guid? aatfId,
            int complianceYear)
        {
            RecipientOrganisationId = recipientOrganisationId;
            OriginatorOrganisationId = originatorOrganisationId;
            AatfId = aatfId;
            ComplianceYear = complianceYear;
        }
    }
}
