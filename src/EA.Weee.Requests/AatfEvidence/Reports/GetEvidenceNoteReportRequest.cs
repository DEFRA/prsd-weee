namespace EA.Weee.Requests.AatfEvidence.Reports
{
    using System;
    using Core.AatfEvidence;

    public class GetEvidenceNoteReportRequest : GetEvidenceReportBaseRequest
    {
        public TonnageToDisplayReportEnum TonnageToDisplay { get; private set; }

        public GetEvidenceNoteReportRequest(Guid? recipientOrganisationId, 
            Guid? originatorOrganisationId,
            Guid? aatfId,
            TonnageToDisplayReportEnum tonnageToDisplay,
            int complianceYear) : base(recipientOrganisationId, 
                                    originatorOrganisationId, 
                                    aatfId, 
                                    complianceYear)
        {
            TonnageToDisplay = tonnageToDisplay;
        }
    }
}
