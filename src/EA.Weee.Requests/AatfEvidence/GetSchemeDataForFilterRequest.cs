namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;

    [Serializable]
    public class GetSchemeDataForFilterRequest : EvidenceEntityIdDisplayNameDataBase
    {
        public Guid? AatfId { get; }

        public RecipientOrTransfer RecipientOrTransfer { get; }

        public List<NoteType> AllowedNoteTypes { get; }

        public GetSchemeDataForFilterRequest(RecipientOrTransfer recipientOrTransfer,  
            Guid? aatfId, 
            int complianceYear, 
            List<NoteStatus> allowedStatuses, 
            List<NoteType> allowedNoteTypes) : 
            base(complianceYear, allowedStatuses)
        {
            Condition.Requires(allowedNoteTypes).IsNotNull();
            Condition.Requires(allowedNoteTypes).IsNotEmpty();

            AatfId = aatfId;
            RecipientOrTransfer = recipientOrTransfer;
            AllowedNoteTypes = allowedNoteTypes;
        }
    }
}
