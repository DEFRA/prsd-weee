namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using DataAccess;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Security;
    using System;
    using System.Threading.Tasks;
    using Core.AatfEvidence;
    using DataAccess.DataAccess;
    using Requests.AatfEvidence;

    public class SetNoteStatusRequestHandler : SaveNoteRequestBase, IRequestHandler<SetNoteStatusRequest, Guid>
    {
        public SetNoteStatusRequestHandler(
            WeeeContext context,
            IUserContext userContext,
            IWeeeAuthorization authorization, 
            ISystemDataDataAccess systemDataDataAccess) : base(context, userContext, authorization, systemDataDataAccess)
        {
        }

        public async Task<Guid> HandleAsync(SetNoteStatusRequest message)
        {
            Authorization.EnsureCanAccessExternalArea();

            var evidenceNote = await EvidenceNote(message.NoteId);

            Authorization.EnsureOrganisationAccess(message.Status == NoteStatus.Submitted
                ? evidenceNote.Organisation.Id
                : evidenceNote.Recipient.Id);

            var currentDate = await SystemDataDataAccess.GetSystemDateTime();

            ValidToSave(evidenceNote.Recipient, evidenceNote.ComplianceYear, currentDate);

            return await UpdateNoteStatus(evidenceNote, message.Status, currentDate, message.Reason);
        }
    }
}
