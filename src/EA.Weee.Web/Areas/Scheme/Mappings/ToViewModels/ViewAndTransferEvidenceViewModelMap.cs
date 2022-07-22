namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using System.Linq;
    using Web.ViewModels.Shared.Mapping;

    public class ViewAndTransferEvidenceViewModelMap : ListOfNotesViewModelBase<SchemeViewAndTransferManageEvidenceSchemeViewModel>, IMap<ViewAndTransferEvidenceViewModelMapTransfer, SchemeViewAndTransferManageEvidenceSchemeViewModel>
    {
        public ViewAndTransferEvidenceViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public SchemeViewAndTransferManageEvidenceSchemeViewModel Map(ViewAndTransferEvidenceViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = MapBase(source.Notes, source.CurrentDate, source.ManageEvidenceNoteViewModel);
            model.OrganisationId = source.OrganisationId;
            model.Scheme = source.Scheme;

            model.DisplayTransferButton = source.Notes.Any(x => x.Status == Core.AatfEvidence.NoteStatus.Approved);

            return model;
        }
    }
}