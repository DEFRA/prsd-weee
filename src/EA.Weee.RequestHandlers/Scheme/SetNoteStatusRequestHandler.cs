namespace EA.Weee.RequestHandlers.Scheme
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Note;
    using Prsd.Core.Mediator;
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

            Guard.ArgumentNotNull(() => evidenceNote, evidenceNote, $"Evidence note {message.NoteId} not found");
            
            authorization.EnsureSchemeAccess(evidenceNote.Recipient.Id);

            string changedBy = userContext.UserId.ToString();

            if (message.Status.Equals(Core.AatfEvidence.NoteStatus.Submitted))
            {
                // status business rule will be checked in that method
                evidenceNote.UpdateStatus(Domain.Evidence.NoteStatus.Approved, changedBy);
            }

            await context.SaveChangesAsync();

            return message.NoteId;
        }
    }
}
