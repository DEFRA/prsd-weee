namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using System.Linq;

    public class ViewAndTransferEvidenceViewModelMap : ListOfSchemeNotesViewModelBase<SchemeViewAndTransferManageEvidenceSchemeViewModel>, IMap<ViewAndTransferEvidenceViewModelMapTransfer, SchemeViewAndTransferManageEvidenceSchemeViewModel>
    {
        public ViewAndTransferEvidenceViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public SchemeViewAndTransferManageEvidenceSchemeViewModel Map(ViewAndTransferEvidenceViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = MapSchemeBase(source.NoteData, source.CurrentDate, source.ManageEvidenceNoteViewModel, source.Scheme);
            model.OrganisationId = source.OrganisationId;
            model.DisplayTransferButton = model.CanSchemeManageEvidence
                                          && source.NoteData.Results.Any(x => x.Status == Core.AatfEvidence.NoteStatus.Approved);
            return model;
        }
    }
}