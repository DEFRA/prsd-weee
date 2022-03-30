namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.Requests.AatfEvidence;
    using Prsd.Core.Mapper;

    public class EditDraftReturnedNotesMapper : IMap<Note, EditDraftReturnedNotesRequest>
    {
        public EditDraftReturnedNotesRequest Map(Note source)
        {
            if (source != null)
            {
                return new EditDraftReturnedNotesRequest(source.Reference, source.Recipient.Id, source.Status, source.WasteType);
            }
            else
            {
                return new EditDraftReturnedNotesRequest();
            }
        }
    }
}
