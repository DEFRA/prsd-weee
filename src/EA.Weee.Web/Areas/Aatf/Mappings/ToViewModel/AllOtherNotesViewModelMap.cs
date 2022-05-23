namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using Prsd.Core.Mapper;
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

            //TODO: place holder map values until the compliance year is put in for AATF
            var model = Map(source.Notes, new DateTime(), null);

            foreach (var evidenceNoteRowViewModel in model.EvidenceNotesDataList)
            {
                evidenceNoteRowViewModel.DisplayViewLink = evidenceNoteRowViewModel.Status.Equals(NoteStatus.Submitted) ||
                                                           evidenceNoteRowViewModel.Status.Equals(NoteStatus.Rejected);
            }

            return model;
        }
    }
}