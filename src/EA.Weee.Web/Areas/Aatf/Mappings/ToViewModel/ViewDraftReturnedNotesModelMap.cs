namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using Core.AatfEvidence;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using Prsd.Core.Mapper;
  
    public class ViewDraftReturnedNotesModelMap : IMap<EditDraftReturnedNotesModel, EditDraftReturnedNote>
    {
        public EditDraftReturnedNote Map(EditDraftReturnedNotesModel source)
        {
            return new EditDraftReturnedNote
            {
                Recipient = source.Recipient,
                ReferenceId = source.ReferenceId,
                Status = source.Status,
                TypeOfWaste = source.WasteType,
                Id = source.Id,
                Type = source.Type,
                SubmittedDate = source.SubmittedDate,
                SubmittedBy = source.SubmittedBy,
                DisplayViewLink = source.DisplayViewLink
            };
        }
    }
}
