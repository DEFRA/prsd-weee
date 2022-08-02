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
    using DataAccess.DataAccess;
    using Factories;

    public class SetNoteStatusRequestHandler : SaveTransferNoteRequestBase, IRequestHandler<SetNoteStatus, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly IUserContext userContext;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public SetNoteStatusRequestHandler(
            WeeeContext context,
            IUserContext userContext,
            IWeeeAuthorization authorization, 
            ISystemDataDataAccess systemDataDataAccess)
        {
            this.authorization = authorization;
            this.systemDataDataAccess = systemDataDataAccess;
            this.context = context;
            this.userContext = userContext;
        }

        public async Task<Guid> HandleAsync(SetNoteStatus message)
        {
            authorization.EnsureCanAccessExternalArea();

            var evidenceNote = await context.Notes.FindAsync(message.NoteId);

            Condition.Requires(evidenceNote).IsNotNull();
            
            authorization.EnsureOrganisationAccess(evidenceNote.Recipient.Id);

            var currentDate = await systemDataDataAccess.GetSystemDateTime();
            var changedBy = userContext.UserId.ToString();

            ValidToSave(evidenceNote.Recipient, evidenceNote.ComplianceYear, currentDate);

            evidenceNote.UpdateStatus(message.Status.ToDomainEnumeration<NoteStatus>(), changedBy, CurrentSystemTimeHelper.GetCurrentTimeBasedOnSystemTime(currentDate), message.Reason);

            await context.SaveChangesAsync();

            return evidenceNote.Id;
        }
    }
}
