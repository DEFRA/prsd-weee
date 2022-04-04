namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.AatfEvidence;
    using EA.Weee.Domain.Evidence;
    using Prsd.Core.Mapper;

    public class EditDraftReturnedNotesMapper : IMap<Note, EvidenceNoteData>
    {
        public EvidenceNoteData Map(Note source)
        {
            return new EvidenceNoteData
            {
                RecipientId = source.Recipient.Id,
                Reference = source.Reference,
                Status = (Core.AatfEvidence.NoteStatus)source.Status.Value,
                WasteType = source.WasteType.HasValue ? (Core.AatfEvidence.WasteType?)source.WasteType.Value : null,
            };
        }
    }
}
