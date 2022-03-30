namespace EA.Weee.Requests.AatfEvidence
{
    using EA.Weee.Domain.Evidence;
    using System;

    public class EditDraftReturnedNotesRequest
    {
        public EditDraftReturnedNotesRequest() 
        {
        }

        public EditDraftReturnedNotesRequest(int referenceId, Guid recipientId, NoteStatus status, WasteType? wasteType)
        {
            this.ReferenceId = referenceId;
            this.RecipientId = recipientId;
            this.Status = status;
            this.WasteType = wasteType;
        }

        public int ReferenceId { get; protected set; }

        public Guid RecipientId { get; protected set; }

        public NoteStatus Status { get; protected set; }

        public WasteType? WasteType { get; protected set; }
    }
}
