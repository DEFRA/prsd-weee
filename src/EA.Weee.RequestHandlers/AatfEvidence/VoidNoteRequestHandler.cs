namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using DataAccess;
    using DataAccess.DataAccess;
    using EA.Weee.Requests.Shared;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Security;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Evidence;
    using Factories;
    using Weee.Security;

    public class VoidNoteRequestHandler : SaveNoteRequestBase, IRequestHandler<VoidNoteRequest, Guid>
    {
        public VoidNoteRequestHandler(
            WeeeContext context,
            IUserContext userContext,
            IWeeeAuthorization authorization, 
            ISystemDataDataAccess systemDataDataAccess) : base(context, userContext, authorization, systemDataDataAccess)
        {
        }

        public async Task<Guid> HandleAsync(VoidNoteRequest message)
        {
            Authorization.EnsureCanAccessInternalArea();
            Authorization.EnsureUserInRole(Roles.InternalAdmin);

            var evidenceNote = await EvidenceNote(message.NoteId);

            if (evidenceNote.Status != Domain.Evidence.NoteStatus.Approved)
            {
                throw new InvalidOperationException($"Cannot void note with id {message.NoteId}");
            }

            if (evidenceNote.NoteType == NoteType.EvidenceNote)
            {
                var transferNotes = evidenceNote.NoteTransferTonnage.Select(e => e.TransferNote).ToList();

                if (transferNotes.Any(t => t.Status == NoteStatus.Approved))
                {
                    throw new InvalidOperationException(
                        $"Cannot void note with id {message.NoteId} as its has approved transfers");
                }
            }

            var currentDate = await SystemDataDataAccess.GetSystemDateTime();

            return await UpdateNoteStatus(evidenceNote, message.Status, CurrentSystemTimeHelper.GetCurrentTimeBasedOnSystemTime(currentDate), message.Reason);
        }
    }
}
