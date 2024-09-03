namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using Prsd.Core.Mapper;
    using Services;
    using ViewModels;
    using Web.ViewModels.Shared.Mapping;

    public class AllOtherNotesViewModelMap : ListOfNotesViewModelBase<AllOtherManageEvidenceNotesViewModel>, IMap<EvidenceNotesViewModelTransfer, AllOtherManageEvidenceNotesViewModel>
    {
        public AllOtherNotesViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public AllOtherManageEvidenceNotesViewModel Map(EvidenceNotesViewModelTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = MapBase(source.NoteData, source.PageNumber, source.PageSize);

            foreach (var evidenceNoteRowViewModel in model.EvidenceNotesDataList)
            {
                evidenceNoteRowViewModel.DisplayViewLink = evidenceNoteRowViewModel.Status.Equals(NoteStatus.Approved) ||
                                                           evidenceNoteRowViewModel.Status.Equals(NoteStatus.Submitted) ||
                                                           evidenceNoteRowViewModel.Status.Equals(NoteStatus.Void) ||
                                                           evidenceNoteRowViewModel.Status.Equals(NoteStatus.Rejected) ||
                                                           evidenceNoteRowViewModel.Status.Equals(NoteStatus.Cancelled);
            }

            return model;
        }
    }
}