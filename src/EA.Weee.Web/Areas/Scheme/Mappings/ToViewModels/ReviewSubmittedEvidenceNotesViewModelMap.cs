namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using Core.Shared;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using Web.ViewModels.Shared.Mapping;

    public class ReviewSubmittedEvidenceNotesViewModelMap : ListOfSchemeNotesViewModelBase<ReviewSubmittedManageEvidenceNotesSchemeViewModel>, IMap<ReviewSubmittedEvidenceNotesViewModelMapTransfer, ReviewSubmittedManageEvidenceNotesSchemeViewModel>
    {
        public ReviewSubmittedEvidenceNotesViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public ReviewSubmittedManageEvidenceNotesSchemeViewModel Map(ReviewSubmittedEvidenceNotesViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = MapSchemeBase(source.NoteData, source.CurrentDate, source.ManageEvidenceNoteViewModel, source.Scheme);
            model.OrganisationId = source.OrganisationId;

            return model;
        }
    }
}