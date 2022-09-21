namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;

    public class ViewAndTransferEvidenceViewModelMap : ListOfSchemeNotesViewModelBase<SchemeViewAndTransferManageEvidenceSchemeViewModel>, IMap<SchemeTabViewModelMapTransfer, SchemeViewAndTransferManageEvidenceSchemeViewModel>
    {
        public ViewAndTransferEvidenceViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public SchemeViewAndTransferManageEvidenceSchemeViewModel Map(SchemeTabViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = MapSchemeBase(source.NoteData, source.Scheme, source.PageNumber, source.PageSize);
            model.OrganisationId = source.OrganisationId;
            model.DisplayTransferButton = model.CanSchemeManageEvidence && source.NoteData.HasApprovedEvidenceNotes;
                                          
            return model;
        }
    }
}