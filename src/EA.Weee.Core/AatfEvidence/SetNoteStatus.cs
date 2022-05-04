namespace EA.Weee.Requests.Note
{
    using System;
    using EA.Weee.Core.AatfEvidence;
    using Prsd.Core.Mediator;

    public class SetNoteStatus : IRequest<Guid>
    {
        public Guid NoteId { get; set; }

        public NoteStatus Status { get; set; }

        public string Reason { get; set; }

        public SetNoteStatus(Guid noteId, NoteStatus status, string reason = null)
        {
            this.NoteId = noteId;
            this.Status = status;
            this.Reason = reason;
        }
    }
}
