namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Aatf;
    using Core.Helpers;
    using CuttingEdge.Conditions;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Domain.Lookup;
    using Domain.Organisation;
    using Factories;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Security;
    using Protocol = Domain.Evidence.Protocol;
    using WasteType = Domain.Evidence.WasteType;

    public class EditEvidenceNoteRequestHandler : SaveEvidenceNoteRequestBase, IRequestHandler<EditEvidenceNoteRequest, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IAatfDataAccess aatfDataAccess;

        public EditEvidenceNoteRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess evidenceDataAccess,
            ISystemDataDataAccess systemDataDataAccess, 
            IGenericDataAccess genericDataAccess, 
            IAatfDataAccess aatfDataAccess)
        {
            this.authorization = authorization;
            this.evidenceDataAccess = evidenceDataAccess;
            this.systemDataDataAccess = systemDataDataAccess;
            this.genericDataAccess = genericDataAccess;
            this.aatfDataAccess = aatfDataAccess;
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

            var recipientOrganisation = await genericDataAccess.GetById<Organisation>(message.RecipientId);
            Condition.Requires(recipientOrganisation).IsNotNull($"Could not find a recipient organisation with Id {message.RecipientId}");

            if (!evidenceNote.Status.Equals(NoteStatus.Draft) && !evidenceNote.Status.Equals(NoteStatus.Returned))
            {
                throw new InvalidOperationException($"Evidence note {evidenceNote.Id} is incorrect state to be edited");
            }

            var complianceYearAatf = await aatfDataAccess.GetAatfByAatfIdAndComplianceYear(evidenceNote.Aatf.AatfId, message.StartDate.Year);

            AatfIsValidToSave(complianceYearAatf, currentDate);

            var tonnageValues = message.TonnageValues.Select(t => new NoteTonnage(
                (WeeeCategory)t.CategoryId,
                t.FirstTonnage,
                t.SecondTonnage)).ToList();

            await evidenceDataAccess.Update(evidenceNote,
                recipientOrganisation,
                message.StartDate,
                message.EndDate,
                message.WasteType != null ? (WasteType?)message.WasteType.Value : null,
                message.Protocol != null ? (Protocol?)message.Protocol.Value : null,
                tonnageValues,
                message.Status.ToDomainEnumeration<NoteStatus>(),
                CurrentSystemTimeHelper.GetCurrentTimeBasedOnSystemTime(currentDate));

            return evidenceNote.Id;
        }

        private bool EnsureTheSchemeNotChanged(Note note, Guid recipientOrganisationId)
        {
            if (note.Status == NoteStatus.Returned)
            {
                return note.RecipientId.Equals(recipientOrganisationId);
            }
            return true;
        }
    }
}