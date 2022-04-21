namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using Web.ViewModels.Shared.Mapping;

    public class ViewAndTransferEvidenceViewModelMap : ListOfNotesViewModelBase<ViewAndTransferEvidenceViewModel>, IMap<ViewAndTransferEvidenceViewModelMapTransfer, ViewAndTransferEvidenceViewModel>
    {
        public ViewAndTransferEvidenceViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public ViewAndTransferEvidenceViewModel Map(ViewAndTransferEvidenceViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = Map(source.Notes);
            model.OrganisationId = source.OrganisationId;
            model.SchemeName = source.SchemeName;

            return model;
        }
    }
}