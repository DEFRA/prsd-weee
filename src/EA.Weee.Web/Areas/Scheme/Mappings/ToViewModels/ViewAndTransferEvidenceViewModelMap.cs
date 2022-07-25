namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using System.Linq;
    using Core.Shared;
    using Web.ViewModels.Shared.Mapping;

    public class ViewAndTransferEvidenceViewModelMap : ListOfSchemeNotesViewModelBase<SchemeViewAndTransferManageEvidenceSchemeViewModel>, IMap<ViewAndTransferEvidenceViewModelMapTransfer, SchemeViewAndTransferManageEvidenceSchemeViewModel>
    {
        public ViewAndTransferEvidenceViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public SchemeViewAndTransferManageEvidenceSchemeViewModel Map(ViewAndTransferEvidenceViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = MapSchemeBase(source.Notes, source.CurrentDate, source.ManageEvidenceNoteViewModel, source.Scheme);
            model.OrganisationId = source.OrganisationId;
            model.DisplayTransferButton = source.Scheme.Status != SchemeStatus.Withdrawn && source.Notes.Any(x => x.Status == Core.AatfEvidence.NoteStatus.Approved);

            return model;
        }
    }
}