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

        public int ReferenceId { get; set; }

        public Guid RecipientId { get; set; }

        public NoteStatus Status { get;  set; }

        public WasteType? WasteType { get; set; }
    }
}
