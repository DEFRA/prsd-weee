namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using System.Linq;
    using Services;

    public class ViewAndTransferEvidenceViewModelMap : ListOfSchemeNotesViewModelBase<SchemeViewAndTransferManageEvidenceSchemeViewModel>, IMap<SchemeTabViewModelMapTransfer, SchemeViewAndTransferManageEvidenceSchemeViewModel>
    {
        public ViewAndTransferEvidenceViewModelMap(IMapper mapper, ConfigurationService configurationService) : base(mapper, configurationService)
        {
        }

        public SchemeViewAndTransferManageEvidenceSchemeViewModel Map(SchemeTabViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = MapSchemeBase(source.NoteData, source.CurrentDate, source.ManageEvidenceNoteViewModel, source.Scheme, source.PageNumber);
            model.OrganisationId = source.OrganisationId;
            model.DisplayTransferButton = model.CanSchemeManageEvidence
                                          && source.NoteData.Results.Any(x => x.Status == Core.AatfEvidence.NoteStatus.Approved);
            return model;
        }
    }
}