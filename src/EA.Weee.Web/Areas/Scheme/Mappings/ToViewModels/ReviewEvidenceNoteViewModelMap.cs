namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModel
{
    using Core.AatfEvidence;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;

    public class ReviewEvidenceNoteViewModelMap : IMap<ViewEvidenceNoteMapTransfer, ReviewEvidenceNoteViewModel>
    {
        private readonly IMapper mapper;

        public ReviewEvidenceNoteViewModelMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ReviewEvidenceNoteViewModel Map(ViewEvidenceNoteMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new ReviewEvidenceNoteViewModel()
            {
                ViewEvidenceNoteViewModel = mapper.Map<ViewEvidenceNoteViewModel>(source)
            };

            //SetSuccessMessage(source.EvidenceNoteData, source.NoteStatus, model);

            return model;
        }

        private void SetSuccessMessage(EvidenceNoteData note, object noteStatus, ViewEvidenceNoteViewModel model)
        {
            if (noteStatus != null)
            {
                if (noteStatus is NoteStatus status)
                {
                    model.SuccessMessage = (status == NoteStatus.Approved ?
                        $"You have successfully approved the evidence note with reference ID {note.Reference}" : 
                        $"You have successfully saved the evidence note with reference ID {note.Reference} as a draft");

                    model.Status = status;
                }
                //else
                //{
                //    model.Status = NoteStatus.Draft;
                //}
            }
        }
    }
}