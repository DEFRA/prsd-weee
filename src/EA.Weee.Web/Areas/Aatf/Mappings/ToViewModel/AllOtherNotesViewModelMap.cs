namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using Prsd.Core.Mapper;
    using ViewModels;
    using Web.ViewModels.Shared.Mapping;

    public class AllOtherNotesViewModelMap : ListOfNotesViewModelBase<AllOtherEvidenceNotesViewModel>, IMap<EvidenceNotesViewModelTransfer, AllOtherEvidenceNotesViewModel>
    {
        public AllOtherNotesViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public AllOtherEvidenceNotesViewModel Map(EvidenceNotesViewModelTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = Map(source.Notes);

            foreach (var evidenceNoteRowViewModel in model.EvidenceNotesDataList)
            {
                evidenceNoteRowViewModel.DisplayViewLink = evidenceNoteRowViewModel.Status.Equals(NoteStatus.Submitted) ||
                                                           evidenceNoteRowViewModel.Status.Equals(NoteStatus.Rejected);
            }

            return model;
        }
    }
}