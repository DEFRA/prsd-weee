namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using Core.AatfEvidence;
    using EA.Prsd.Core.Mediator;

    public class VoidTransferNoteRequest : SetNoteStatusBase, IRequest<Guid>
    {
        public VoidTransferNoteRequest(Guid noteId, string reason = null) : base(noteId, NoteStatus.Void, reason)
        {
        }
    }
}
