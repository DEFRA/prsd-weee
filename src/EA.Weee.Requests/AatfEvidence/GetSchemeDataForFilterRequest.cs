namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;

    [Serializable]
    public class GetSchemeDataForFilterRequest : EvidenceEntityIdDisplayNameDataBase
    {
        public Guid? AatfId { get; private set; }

        public RecipientOrTransfer RecipientOrTransfer { get; private set; }

        public GetSchemeDataForFilterRequest(RecipientOrTransfer recipientOrTransfer,  Guid? aatfId, int complianceYear, List<NoteStatus> allowedStatuses) : 
            base(complianceYear, allowedStatuses)
        {
            AatfId = aatfId;
            RecipientOrTransfer = recipientOrTransfer;
        }
    }
}
