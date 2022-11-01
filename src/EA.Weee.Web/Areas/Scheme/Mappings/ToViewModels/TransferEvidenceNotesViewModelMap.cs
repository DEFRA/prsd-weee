namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System.Collections.Generic;
    using Core.Shared.Paging;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using Services.Caching;
    using ViewModels;
    using Web.ViewModels.Shared;
    using System.Linq;
    using Core.Helpers;
    using Web.ViewModels.Shared.Mapping;

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

            if (source.SelectedNotes != null)
            {
                model.EvidenceNotesDataList = new List<ViewEvidenceNoteViewModel>();
                // here check if before adding any of the selected notes to the selected table they are still available as the category selection may have changed and the notes may may not have a category amount
                if (source.Request != null)
                {
                    foreach (var evidenceNoteData in source.SelectedNotes.Results.OrderByDescending(n => n.Reference))
                    {
                        if (evidenceNoteData.EvidenceTonnageData.Where(e => e.Received.HasValue)
                            .Select(e => e.CategoryId.ToInt()).Intersect(source.Request.CategoryIds).Any())
                        {
                            model.EvidenceNotesDataList.Add(Mapper.Map<ViewEvidenceNoteViewModel>(
                                new ViewEvidenceNoteMapTransfer(evidenceNoteData, null, false, null)
                                {
                                    IncludeAllCategories = false
                                }));
                        }
                        else
                        {
                            source.Request.DeselectedEvidenceNoteIds.Add(evidenceNoteData.Id);
                            source.Request.EvidenceNoteIds.Remove(evidenceNoteData.Id);
                        }
                    }

                    // here check if any of the currently selected evidence notes in the session available are no longer available.
                    for (var idx = source.Request.EvidenceNoteIds.Count() - 1; idx >= 0; idx--)
                    {
                        if (source.SelectedNotes.Results.All(e => e.Id != source.Request.EvidenceNoteIds.ElementAt(idx)))
                        {
                            source.Request.DeselectedEvidenceNoteIds.Add(source.Request.EvidenceNoteIds.ElementAt(idx));
                            source.Request.EvidenceNoteIds.Remove(source.Request.EvidenceNoteIds.ElementAt(idx));
                        }
                    }
                }
            }

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