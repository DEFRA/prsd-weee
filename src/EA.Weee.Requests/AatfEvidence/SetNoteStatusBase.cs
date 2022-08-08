namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using Core.AatfEvidence;

    public class SetNoteStatusBase
    {
        public SetNoteStatusBase(Guid noteId, NoteStatus status, string reason = null)
        {
            this.NoteId = noteId;
            this.Status = status;
            this.Reason = reason;
        }

        public Guid NoteId { get; private set; }

        public NoteStatus Status { get; private set; }

        public string Reason { get; private set; }
    }
}
