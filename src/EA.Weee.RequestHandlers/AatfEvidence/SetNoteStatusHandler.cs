namespace EA.Weee.RequestHandlers.AatfEvidence
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

    public class SetNoteStatusHandler : IRequestHandler<SetNoteStatus, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;
        private readonly WeeeContext context;
        private readonly IUserContext userContext;

        public SetNoteStatusHandler(
                                    WeeeContext context, 
                                    IWeeeAuthorization authorization,
                                    IEvidenceDataAccess evidenceDataAccess,
                                    IUserContext userContext,
                                    IMapper mapper)
        {
            this.authorization = authorization;
            this.evidenceDataAccess = evidenceDataAccess;
            this.mapper = mapper;
            this.context = context;
            this.userContext = userContext;
        }

        public async Task<Guid> HandleAsync(SetNoteStatus message)
        {
            authorization.EnsureCanAccessExternalArea();

            var evidenceNote = await context.Notes.FindAsync(message.NoteId);

            Guard.ArgumentNotNull(() => evidenceNote, evidenceNote, $"Evidence note {message.NoteId} not found");
            
            authorization.CheckSchemeAccess(evidenceNote.Recipient.Id);

            string changedBy = userContext.UserId.ToString();

            if (message.Status.Equals(Core.AatfEvidence.NoteStatus.Submitted))
            {
                evidenceNote.UpdateStatus(Domain.Evidence.NoteStatus.Approved, changedBy);
            }

            await context.SaveChangesAsync();

            return message.NoteId;
        }
    }
}
