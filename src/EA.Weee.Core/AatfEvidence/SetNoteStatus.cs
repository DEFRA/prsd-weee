namespace EA.Weee.Requests.Note
{
    using Core.Shared;
    using EA.Weee.Core.AatfEvidence;
    using Prsd.Core.Mediator;
    using System;

    public class SetNoteStatus : IRequest<Guid>
    {
        public Guid NoteId { get; set; }

        public NoteStatus Status { get; set; }

        public SetNoteStatus(Guid noteId, NoteStatus status)
        {
            this.NoteId = noteId;
            this.Status = status;
        }
    }
}
