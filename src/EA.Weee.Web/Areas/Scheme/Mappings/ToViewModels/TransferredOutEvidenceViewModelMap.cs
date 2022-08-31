namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using Services;

    public class TransferredOutEvidenceViewModelMap : ListOfSchemeNotesViewModelBase<TransferredOutEvidenceNotesSchemeViewModel>, IMap<SchemeTabViewModelMapTransfer, TransferredOutEvidenceNotesSchemeViewModel>
    {
        public TransferredOutEvidenceViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public TransferredOutEvidenceNotesSchemeViewModel Map(SchemeTabViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = MapSchemeBase(source.NoteData, source.CurrentDate, source.ManageEvidenceNoteViewModel, source.Scheme, source.PageNumber, source.PageSize);
            model.OrganisationId = source.OrganisationId;
           
            foreach (var evidenceNoteRowViewModel in model.EvidenceNotesDataList)
            {
                evidenceNoteRowViewModel.DisplayEditLink = (evidenceNoteRowViewModel.Status == NoteStatus.Draft ||
                                                           evidenceNoteRowViewModel.Status == NoteStatus.Returned) && 
                                                           model.CanSchemeManageEvidence;
            }

            return model;
        }
    }
}