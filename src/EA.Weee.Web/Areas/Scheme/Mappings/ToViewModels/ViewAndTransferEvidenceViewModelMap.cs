namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using System.Linq;

    public class ViewAndTransferEvidenceViewModelMap : IMap<ViewAndTransferEvidenceViewModelMapTransfer, ViewAndTransferEvidenceViewModel>
    {
        private readonly IMapper mapper;

        public ViewAndTransferEvidenceViewModelMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ViewAndTransferEvidenceViewModel Map(ViewAndTransferEvidenceViewModelMapTransfer source)
        {
            var model = new ViewAndTransferEvidenceViewModel();

            model.OrganisationId = source.OrganisationId;
            model.SchemeName = source.SchemeName;

            if (source != null && source.Notes.Any())
            {
                foreach (var res in source.Notes)
                {
                    var aatfName = res.AatfData != null ? res.AatfData.Name : string.Empty;

                    model.EvidenceNotesDataList.Add(mapper.Map<EditDraftReturnedNote>
                        (new EditDraftReturnedNotesModel(res.Reference, res.SchemeData.SchemeName, res.Status, res.WasteType, res.Id, res.Type, res.SubmittedDate, aatfName)));
                }
            }

            return model;
        }
    }
}