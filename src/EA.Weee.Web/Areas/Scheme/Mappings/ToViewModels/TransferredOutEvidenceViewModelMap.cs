namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;

    public class TransferredOutEvidenceViewModelMap : ListOfSchemeNotesViewModelBase<TransferredOutEvidenceNotesSchemeViewModel>, IMap<SchemeTabViewModelMapTransfer, TransferredOutEvidenceNotesSchemeViewModel>
    {
        public TransferredOutEvidenceViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public TransferredOutEvidenceNotesSchemeViewModel Map(SchemeTabViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = MapSchemeBase(source.NoteData, source.Scheme, source.CurrentDate, source.SelectedComplianceYear, source.PageNumber, source.PageSize);
            model.OrganisationId = source.OrganisationId;
           
            foreach (var evidenceNoteRowViewModel in model.EvidenceNotesDataList)
            {
                evidenceNoteRowViewModel.DisplayEditLink = (evidenceNoteRowViewModel.Status == NoteStatus.Draft ||
                                                           evidenceNoteRowViewModel.Status == NoteStatus.Returned) && 
                                                           model.CanSchemeManageEvidence;

                evidenceNoteRowViewModel.DisplayCancelLink = evidenceNoteRowViewModel.Status == NoteStatus.Returned && model.CanSchemeManageEvidence;
            }

            return model;
        }
    }
}