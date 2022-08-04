namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using Core.AatfEvidence;
    using EA.Prsd.Core.Mediator;

    public class SetNoteStatusRequest : SetNoteStatusBase, IRequest<Guid>
    {
        public SetNoteStatusRequest(Guid noteId, NoteStatus status, string reason = null) : base(noteId, status, reason)
        {
        }
    }
}
