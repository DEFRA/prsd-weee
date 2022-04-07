namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;

    public class SchemeDataToManageEvidenceNoteViewModelMap : IMap<SchemeDataToManageEvidenceNoteViewModelMapTransfer, ManageEvidenceNoteViewModel>
    {
        public ManageEvidenceNoteViewModel Map(SchemeDataToManageEvidenceNoteViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new ReviewSubmittedEvidenceNotesViewModel
            {
                OrganisationId = source.OrganisationId,
                OrganisationName = source.OrganisationName,
            };

            return model;
        }
    }
}
