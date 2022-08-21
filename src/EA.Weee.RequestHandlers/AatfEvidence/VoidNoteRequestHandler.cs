namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using DataAccess;
    using DataAccess.DataAccess;
    using EA.Weee.Requests.Shared;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Security;
    using System;
    using System.Threading.Tasks;
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

            var currentDate = await SystemDataDataAccess.GetSystemDateTime();

            return await UpdateNoteStatus(evidenceNote, message.Status, currentDate, message.Reason);
        }
    }
}
