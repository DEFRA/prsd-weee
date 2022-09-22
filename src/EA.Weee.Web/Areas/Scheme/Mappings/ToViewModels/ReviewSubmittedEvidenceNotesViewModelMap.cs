namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;

    public class ReviewSubmittedEvidenceNotesViewModelMap : ListOfSchemeNotesViewModelBase<ReviewSubmittedManageEvidenceNotesSchemeViewModel>, IMap<SchemeTabViewModelMapTransfer, ReviewSubmittedManageEvidenceNotesSchemeViewModel>
    {
        public ReviewSubmittedEvidenceNotesViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public ReviewSubmittedManageEvidenceNotesSchemeViewModel Map(SchemeTabViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = MapSchemeBase(source.NoteData, source.Scheme, source.CurrentDate, source.SelectedComplianceYear, source.PageNumber, source.PageSize);

            model.OrganisationId = source.OrganisationId;

            return model;
        }
    }
}