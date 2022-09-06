namespace EA.Weee.Requests.AatfEvidence.Reports
{
    using System;
    using Core.AatfEvidence;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetEvidenceNoteReportRequest : IRequest<CSVFileData>
    {
        public Guid? RecipientOrganisationId { get; private set; }

        public Guid? OriginatorOrganisationId { get; private set; }

        public TonnageToDisplayReportEnum TonnageToDisplay { get; private set; }

        public GetEvidenceNoteReportRequest(Guid? recipientOrganisationId, 
            Guid? originatorOrganisationId,
            TonnageToDisplayReportEnum tonnageToDisplay)
        {
            RecipientOrganisationId = recipientOrganisationId;
            OriginatorOrganisationId = originatorOrganisationId;
            TonnageToDisplay = tonnageToDisplay;
        }
    }
}
