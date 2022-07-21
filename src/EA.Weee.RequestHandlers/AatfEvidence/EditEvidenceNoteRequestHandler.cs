namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Helpers;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Domain.Lookup;
    using Factories;
    using Prsd.Core;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Security;
    using Protocol = Domain.Evidence.Protocol;
    using WasteType = Domain.Evidence.WasteType;

    public class EditEvidenceNoteRequestHandler : SaveEvidenceNoteRequestBase, IRequestHandler<EditEvidenceNoteRequest, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly ISchemeDataAccess schemeDataAccess;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public EditEvidenceNoteRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess evidenceDataAccess,
            ISchemeDataAccess schemeDataAccess, 
            ISystemDataDataAccess systemDataDataAccess)
        {
            this.authorization = authorization;
            this.evidenceDataAccess = evidenceDataAccess;
            this.schemeDataAccess = schemeDataAccess;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<Guid> HandleAsync(EditEvidenceNoteRequest message)
        {
            authorization.EnsureCanAccessExternalArea();

            var evidenceNote = await evidenceDataAccess.GetNoteById(message.Id);

            authorization.EnsureOrganisationAccess(evidenceNote.OrganisationId);

            var currentDate = await systemDataDataAccess.GetSystemDateTime();

            if (!EnsureTheSchemeNotChanged(evidenceNote, message.RecipientId))
            {
                throw new InvalidOperationException($"Evidence note {evidenceNote.Id} has incorrect Recipient Id to be saved");
            }
            
            var scheme = await schemeDataAccess.GetSchemeOrDefault(message.RecipientId);

            Guard.ArgumentNotNull(() => scheme, scheme, $"Scheme {message.RecipientId} not found");

            if (!evidenceNote.Status.Equals(NoteStatus.Draft) && !evidenceNote.Status.Equals(NoteStatus.Returned))
            {
                throw new InvalidOperationException($"Evidence note {evidenceNote.Id} is incorrect state to be edited");
            }

            AatfIsValidToSave(evidenceNote.Aatf, currentDate);

            var tonnageValues = message.TonnageValues.Select(t => new NoteTonnage(
                (WeeeCategory)t.CategoryId,
                t.FirstTonnage,
                t.SecondTonnage)).ToList();

            await evidenceDataAccess.Update(evidenceNote,
                scheme.Organisation,
                message.StartDate,
                message.EndDate,
                message.WasteType != null ? (WasteType?)message.WasteType.Value : null,
                message.Protocol != null ? (Protocol?)message.Protocol.Value : null,
                tonnageValues,
                message.Status.ToDomainEnumeration<NoteStatus>(),
                CurrentSystemTimeHelper.GetCurrentTimeBasedOnSystemTime(currentDate));

            return evidenceNote.Id;
        }

        private bool EnsureTheSchemeNotChanged(Note note, Guid schemeIdFromModel)
        {
            if (note.Status == NoteStatus.Returned)
            {
                return note.RecipientId.Equals(schemeIdFromModel);
            }
            return true;
        }
    }
}