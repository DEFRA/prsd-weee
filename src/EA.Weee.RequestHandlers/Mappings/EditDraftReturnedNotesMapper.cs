namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Weee.Domain.Evidence;
    using EA.Weee.Requests.AatfEvidence;
    using Prsd.Core.Mapper;

    public class EditDraftReturnedNotesMapper : IMap<Note, EditDraftReturnedNotesRequest>
    {
        public EditDraftReturnedNotesRequest Map(Note source)
        {
            return new EditDraftReturnedNotesRequest
            {
                RecipientId = source.Recipient.Id,
                ReferenceId = source.Reference,
                Status = source.Status.DisplayName.ToString(),
                WasteType = source.WasteType.HasValue ? source.WasteType.Value.ToString() : string.Empty,
            };
        }
    }
}
