namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class AllOtherNotesViewModelMap : ListOfNotesViewModelBase<AllOtherEvidenceNotesViewModel>, IMap<EditDraftReturnNotesViewModelTransfer, AllOtherEvidenceNotesViewModel>
    {
        public AllOtherNotesViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public AllOtherEvidenceNotesViewModel Map(EditDraftReturnNotesViewModelTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = Map(source.Notes);

            foreach (var evidenceNoteRowViewModel in model.ListOfNotes)
            {
                evidenceNoteRowViewModel.DisplayViewLink = evidenceNoteRowViewModel.Status.Equals(NoteStatus.Submitted);
            }
            
            return model;
        }
    }
}