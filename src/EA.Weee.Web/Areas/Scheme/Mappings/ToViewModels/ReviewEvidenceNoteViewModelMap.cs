namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels.ManageEvidenceNotes;
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
                ViewEvidenceNoteViewModel = mapper.Map<ViewEvidenceNoteViewModel>(source),
                OrganisationId = source.SchemeId
            };

            model.ViewEvidenceNoteViewModel.DisplayH2Title = true;

            return model;
        }
    }
}