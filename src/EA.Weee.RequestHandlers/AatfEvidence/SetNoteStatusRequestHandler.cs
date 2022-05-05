namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using Core.Helpers;
    using CuttingEdge.Conditions;
    using DataAccess;
    using Domain.Evidence;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Note;
    using Security;
    using System;
    using System.Threading.Tasks;

    public class SetNoteStatusRequestHandler : IRequestHandler<SetNoteStatus, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly IUserContext userContext;

        public SetNoteStatusRequestHandler(
            WeeeContext context,
            IUserContext userContext,
            IWeeeAuthorization authorization)
        {
            this.authorization = authorization;
            this.context = context;
            this.userContext = userContext;
        }

        public async Task<Guid> HandleAsync(SetNoteStatus message)
        {
            authorization.EnsureCanAccessExternalArea();

            var evidenceNote = await context.Notes.FindAsync(message.NoteId);

            Condition.Requires(evidenceNote).IsNotNull();
            
            authorization.EnsureSchemeAccess(evidenceNote.Recipient.Id);

            string changedBy = userContext.UserId.ToString();
            evidenceNote.UpdateStatus(message.Status.ToDomainEnumeration<NoteStatus>(), changedBy, message.Reason);

            await context.SaveChangesAsync();

            return evidenceNote.Id;
        }
    }
}
