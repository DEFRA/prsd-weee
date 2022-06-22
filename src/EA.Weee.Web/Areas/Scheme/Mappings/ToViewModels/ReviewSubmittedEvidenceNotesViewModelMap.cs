namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using Prsd.Core;
    using Web.ViewModels.Shared.Mapping;

    public class ReviewSubmittedEvidenceNotesViewModelMap : ListOfNotesViewModelBase<ReviewSubmittedManageEvidenceNotesSchemeViewModel>, IMap<ReviewSubmittedEvidenceNotesViewModelMapTransfer, ReviewSubmittedManageEvidenceNotesSchemeViewModel>
    {
        public ReviewSubmittedEvidenceNotesViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public ReviewSubmittedManageEvidenceNotesSchemeViewModel Map(ReviewSubmittedEvidenceNotesViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = MapBase(source.Notes, source.CurrentDate, source.ManageEvidenceNoteViewModel);
            model.OrganisationId = source.OrganisationId;
            model.SchemeName = source.SchemeName;

            return model;
        }
    }
}