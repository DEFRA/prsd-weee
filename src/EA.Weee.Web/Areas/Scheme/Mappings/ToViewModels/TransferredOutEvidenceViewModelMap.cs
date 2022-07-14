namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using Web.ViewModels.Shared.Mapping;

    public class TransferredOutEvidenceViewModelMap : ListOfNotesViewModelBase<TransferredOutEvidenceNotesSchemeViewModel>, IMap<TransferredOutEvidenceNotesViewModelMapTransfer, TransferredOutEvidenceNotesSchemeViewModel>
    {
        public TransferredOutEvidenceViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public TransferredOutEvidenceNotesSchemeViewModel Map(TransferredOutEvidenceNotesViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = MapBase(source.Notes, source.CurrentDate, source.ManageEvidenceNoteViewModel);
            model.OrganisationId = source.OrganisationId;
            model.SchemeName = source.SchemeName;

            foreach (var evidenceNoteRowViewModel in model.EvidenceNotesDataList)
            {
                evidenceNoteRowViewModel.DisplayViewLink = evidenceNoteRowViewModel.Status.Equals(NoteStatus.Draft) ||
                                                           evidenceNoteRowViewModel.Status.Equals(NoteStatus.Submitted) ||
                                                           evidenceNoteRowViewModel.Status.Equals(NoteStatus.Rejected) ||
                                                           evidenceNoteRowViewModel.Status.Equals(NoteStatus.Returned) ||
                                                           evidenceNoteRowViewModel.Status.Equals(NoteStatus.Approved);
                evidenceNoteRowViewModel.DisplayEditLink = evidenceNoteRowViewModel.Status.Equals(NoteStatus.Draft);
            }

            return model;
        }
    }
}