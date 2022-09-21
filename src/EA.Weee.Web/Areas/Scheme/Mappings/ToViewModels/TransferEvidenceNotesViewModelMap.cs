namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using Core.Shared.Paging;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using Services.Caching;
    using ViewModels;
    using Web.ViewModels.Shared;

    public class TransferEvidenceNotesViewModelMap : TransferEvidenceMapBase<TransferEvidenceNotesViewModel>, IMap<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>
    {
        public TransferEvidenceNotesViewModelMap(IMapper mapper, IWeeeCache cache, IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel> transferNoteMapper) : base(mapper, cache, transferNoteMapper)
        {
        }

        public TransferEvidenceNotesViewModel Map(TransferEvidenceNotesViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();
            Condition.Requires(source.AvailableNotes).IsNotNull();

            var model = MapBaseProperties(source);

            foreach (var evidenceNoteData in source.AvailableNotes.Results)
            {
                model.EvidenceNotesDataListPaged.Add(Mapper.Map<EvidenceNoteRowViewModel>(evidenceNoteData));
            }

            model.EvidenceNotesDataListPaged = 
                model.EvidenceNotesDataListPaged.ToPagedList(source.PageNumber - 1, source.PageSize, source.AvailableNotes.NoteCount) as PagedList<EvidenceNoteRowViewModel>;

            model.SearchRef = source.SearchRef;
            model.PageNumber = source.PageNumber;

            return model;
        }
    }
}