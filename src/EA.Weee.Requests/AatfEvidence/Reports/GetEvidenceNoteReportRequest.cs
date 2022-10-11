namespace EA.Weee.Requests.AatfEvidence.Reports
{
    using System;
    using Core.AatfEvidence;

    public class GetEvidenceNoteReportRequest : GetEvidenceReportBaseRequest
    {
        public TonnageToDisplayReportEnum TonnageToDisplay { get; private set; }

        public bool InternalRequest { get; private set; }

        public GetEvidenceNoteReportRequest(Guid? recipientOrganisationId,
            Guid? aatfId,
            TonnageToDisplayReportEnum tonnageToDisplay,
            int complianceYear) : base(recipientOrganisationId, 
                                    null, 
                                    aatfId, 
                                    complianceYear)
        {
            if (recipientOrganisationId.HasValue && aatfId.HasValue)
            {
                throw new InvalidOperationException(
                    "GetEvidenceNoteReportRequest recipient and aatf cannot both be specified");
            }

            if (!recipientOrganisationId.HasValue && !aatfId.HasValue)
            {
                InternalRequest = true;
            }

            TonnageToDisplay = tonnageToDisplay;
        }
    }
}
