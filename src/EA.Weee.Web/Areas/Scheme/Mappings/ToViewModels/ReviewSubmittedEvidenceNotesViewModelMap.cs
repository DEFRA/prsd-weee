namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class ReviewSubmittedEvidenceNotesViewModelMap : IMap<ReviewSubmittedEvidenceNotesViewModelMapTransfer, ReviewSubmittedEvidenceNotesViewModel>
    {
        private readonly IMapper mapper;

        public ReviewSubmittedEvidenceNotesViewModelMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ReviewSubmittedEvidenceNotesViewModel Map(ReviewSubmittedEvidenceNotesViewModelMapTransfer source)
        {
            var model = new ReviewSubmittedEvidenceNotesViewModel();

            if (source != null && source.Notes.Any())
            {
                foreach (var res in source.Notes)
                {
                    model.EvidenceNotesDataList.Add(mapper.Map<EditDraftReturnedNote>
                        (new EditDraftReturnedNotesModel(res.Reference, res.SchemeData.SchemeName, res.Status, res.WasteType)));
                }
            }

            return model;
        }
    }
}