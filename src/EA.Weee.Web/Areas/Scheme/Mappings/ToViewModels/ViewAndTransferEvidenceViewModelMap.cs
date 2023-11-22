namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using System.Linq;

    public class ViewAndTransferEvidenceViewModelMap : ListOfSchemeNotesViewModelBase<SchemeViewAndTransferManageEvidenceSchemeViewModel>, IMap<SchemeTabViewModelMapTransfer, SchemeViewAndTransferManageEvidenceSchemeViewModel>
    {
        public ViewAndTransferEvidenceViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public SchemeViewAndTransferManageEvidenceSchemeViewModel Map(SchemeTabViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = MapSchemeBase(source.NoteData, source.Scheme, source.CurrentDate, source.SelectedComplianceYear, source.PageNumber, source.PageSize);

            model.OrganisationId = source.OrganisationId;
            model.DisplayTransferButton = model.CanSchemeManageEvidence && source.NoteData.HasApprovedEvidenceNotes && (source.NoteData.Results.Sum(x => x.TotalReceivedAvailable) > 0);

            return model;
        }
    }
}