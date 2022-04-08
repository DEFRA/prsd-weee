namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AatfReturn;
    using AatfReturn.Internal;
    using Core.Helpers;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Domain.Lookup;
    using Domain.Organisation;
    using Prsd.Core;
    using Prsd.Core.Domain;
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

            Guard.ArgumentNotNull(() => evidenceNote, evidenceNote, $"Evidence note {message.Id} not found");

            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var scheme = await schemeDataAccess.GetSchemeOrDefault(message.RecipientId);

            Guard.ArgumentNotNull(() => scheme, scheme, $"Scheme {message.RecipientId} not found");

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
    }
}