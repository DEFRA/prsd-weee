namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using Services;

    public class ReviewSubmittedEvidenceNotesViewModelMap : ListOfSchemeNotesViewModelBase<ReviewSubmittedManageEvidenceNotesSchemeViewModel>, IMap<SchemeTabViewModelMapTransfer, ReviewSubmittedManageEvidenceNotesSchemeViewModel>
    {
        public ReviewSubmittedEvidenceNotesViewModelMap(IMapper mapper, ConfigurationService configurationService) : base(mapper, configurationService)
        {
        }

        public ReviewSubmittedManageEvidenceNotesSchemeViewModel Map(SchemeTabViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = MapSchemeBase(source.NoteData, source.CurrentDate, source.ManageEvidenceNoteViewModel, source.Scheme, source.PageNumber);
            model.OrganisationId = source.OrganisationId;

            return model;
        }
    }
}