namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using DataAccess;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Security;
    using System;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Requests.AatfEvidence;
    using Weee.Security;

    public class VoidTransferNoteRequestHandler : SaveNoteRequestBase, IRequestHandler<VoidTransferNoteRequest, Guid>
    {
        public VoidTransferNoteRequestHandler(
            WeeeContext context,
            IUserContext userContext,
            IWeeeAuthorization authorization, 
            ISystemDataDataAccess systemDataDataAccess) : base(context, userContext, authorization, systemDataDataAccess)
        {
        }

        public async Task<Guid> HandleAsync(VoidTransferNoteRequest message)
        {
            Authorization.EnsureCanAccessInternalArea();
            Authorization.EnsureUserInRole(Roles.InternalAdmin);

            var evidenceNote = await EvidenceNote(message.NoteId);

            if (evidenceNote.Status != Domain.Evidence.NoteStatus.Approved || evidenceNote.NoteType != Domain.Evidence.NoteType.TransferNote)
            {
                throw new InvalidOperationException($"Cannot void note with id {message.NoteId}");
            }

            var currentDate = await SystemDataDataAccess.GetSystemDateTime();

            return await UpdateNoteStatus(evidenceNote, message.Status, currentDate, message.Reason);
        }
    }
}
