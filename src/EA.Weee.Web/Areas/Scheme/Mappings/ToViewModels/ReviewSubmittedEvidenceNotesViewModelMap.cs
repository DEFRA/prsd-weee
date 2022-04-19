namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System.Collections.Generic;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using System.Linq;

    public class ReviewSubmittedEvidenceNotesViewModelMap : IMap<ReviewSubmittedEvidenceNotesViewModelMapTransfer, ReviewSubmittedEvidenceNotesViewModel>
    {
        private readonly IMapper mapper;

        public ReviewSubmittedEvidenceNotesViewModelMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ReviewSubmittedEvidenceNotesViewModel Map(ReviewSubmittedEvidenceNotesViewModelMapTransfer source)
        {
            var model = new ReviewSubmittedEvidenceNotesViewModel
            {
                OrganisationId = source.OrganisationId,
                SchemeName = source.SchemeName,
                EvidenceNotesDataList = mapper.Map<List<EvidenceNoteRowViewModel>>(source.Notes)
            };

            return model;
        }
    }
}