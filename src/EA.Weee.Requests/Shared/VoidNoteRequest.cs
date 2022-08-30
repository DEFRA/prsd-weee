namespace EA.Weee.Requests.Shared
{
    using System;
    using Core.AatfEvidence;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Requests.AatfEvidence;

    public class VoidNoteRequest : SetNoteStatusBase, IRequest<Guid>
    {
        public VoidNoteRequest(Guid noteId, string reason = null) : base(noteId, NoteStatus.Void, reason)
        {
        }
    }
}
