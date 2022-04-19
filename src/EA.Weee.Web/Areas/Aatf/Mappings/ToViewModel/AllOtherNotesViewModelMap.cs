namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Collections.Generic;
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

            var model = new AllOtherEvidenceNotesViewModel
            {
                ListOfNotes = mapper.Map<List<EvidenceNoteRowViewModel>>(source.Notes)
            };

            foreach (var evidenceNoteRowViewModel in model.ListOfNotes)
            {
                evidenceNoteRowViewModel.DisplayViewLink = evidenceNoteRowViewModel.Status.Equals(NoteStatus.Submitted);
            }
            
            return model;
        }
    }
}