namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Helpers;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Domain.Lookup;
    using Prsd.Core;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Security;
    using Protocol = Domain.Evidence.Protocol;
    using WasteType = Domain.Evidence.WasteType;

    public class EditEvidenceNoteRequestHandler : IRequestHandler<EditEvidenceNoteRequest, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly ISchemeDataAccess schemeDataAccess;

        public EditEvidenceNoteRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess evidenceDataAccess,
            ISchemeDataAccess schemeDataAccess)
        {
            this.authorization = authorization;
            this.evidenceDataAccess = evidenceDataAccess;
            this.schemeDataAccess = schemeDataAccess;
        }

        public async Task<Guid> HandleAsync(EditEvidenceNoteRequest message)
        {
            authorization.EnsureCanAccessExternalArea();

            var evidenceNote = await evidenceDataAccess.GetNoteById(message.Id);

            authorization.EnsureOrganisationAccess(evidenceNote.OrganisationId);

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

            var tonnageValues = message.TonnageValues.Select(t => new NoteTonnage(
                (WeeeCategory)t.CategoryId,
                t.FirstTonnage,
                t.SecondTonnage)).ToList();

            await evidenceDataAccess.Update(evidenceNote,
                scheme,
                message.StartDate,
                message.EndDate,
                message.WasteType != null ? (WasteType?)message.WasteType.Value : null,
                message.Protocol != null ? (Protocol?)message.Protocol.Value : null,
                tonnageValues,
                message.Status.ToDomainEnumeration<NoteStatus>());

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