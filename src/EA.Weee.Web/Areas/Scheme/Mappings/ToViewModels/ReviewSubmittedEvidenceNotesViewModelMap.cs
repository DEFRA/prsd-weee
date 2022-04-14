namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
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
            var model = new ReviewSubmittedEvidenceNotesViewModel();

            model.OrganisationId = source.OrganisationId;
            model.SchemeName = source.SchemeName;

            if (source != null && source.Notes.Any())
            {
                foreach (var res in source.Notes)
                {
                    string aatfOrgName;

                    if (res.AatfData != null)
                    {
                        aatfOrgName = res.AatfData.Organisation != null ? res.AatfData.Organisation.Name : string.Empty;
                    } 
                    else
                    {
                        aatfOrgName = string.Empty;
                    }

                    model.EvidenceNotesDataList.Add(mapper.Map<EditDraftReturnedNote>
                        (new EditDraftReturnedNotesModel(res.Reference, res.SchemeData.SchemeName, res.Status, res.WasteType, res.Id, res.Type, res.SubmittedDate, aatfOrgName)));
                }
            }

            return model;
        }
    }
}