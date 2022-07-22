namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System;
    using System.Linq;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using Web.ViewModels.Shared.Mapping;

    public class ViewAndTransferEvidenceViewModelMap : ListOfNotesViewModelBase<SchemeViewAndTransferManageEvidenceSchemeViewModel>, IMap<ViewAndTransferEvidenceViewModelMapTransfer, SchemeViewAndTransferManageEvidenceSchemeViewModel>
    {
        public ViewAndTransferEvidenceViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public SchemeViewAndTransferManageEvidenceSchemeViewModel Map(ViewAndTransferEvidenceViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = MapBase(source.NoteData, source.CurrentDate, source.ManageEvidenceNoteViewModel);
            model.OrganisationId = source.OrganisationId;
            model.SchemeName = source.SchemeName;

            model.DisplayTransferButton = source.NoteData.Results.Any(x => x.Status == Core.AatfEvidence.NoteStatus.Approved);

            return model;
        }
    }
}