namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Linq;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class AllOtherNotesViewModelMap : IMap<EditDraftReturnNotesViewModelTransfer, AllOtherEvidenceNotesViewModel>
    {
        private readonly IMapper mapper;

        public AllOtherNotesViewModelMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public AllOtherEvidenceNotesViewModel Map(EditDraftReturnNotesViewModelTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new AllOtherEvidenceNotesViewModel();

            if (source != null && source.Notes.Any())
            {
                foreach (var res in source.Notes)
                {
                    model.ListOfNotes.Add(mapper.Map<EditDraftReturnedNote>(new EditDraftReturnedNotesModel(res.Reference, res.SchemeData.SchemeName, res.Status, res.WasteType, res.Id, res.Type, res.SubmittedDate, string.Empty, res.Status.Equals(NoteStatus.Submitted)))); 
                }
            }
            return model;
        }
    }
}