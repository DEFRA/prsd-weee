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

            //if (source.SelectedNotes != null)
            //{
            //    model.EvidenceNotesDataList = new List<ViewEvidenceNoteViewModel>();
            //    foreach (var evidenceNoteData in source.SelectedNotes.Results.OrderByDescending(n => n.Reference))
            //    {
            //        if (evidenceNoteData.EvidenceTonnageData.Any(e => source.Request.CategoryIds.Contains(e.CategoryId.ToInt())))
            //        {
            //            model.EvidenceNotesDataList.Add(Mapper.Map<ViewEvidenceNoteViewModel>(
            //                new ViewEvidenceNoteMapTransfer(evidenceNoteData, null, false, null)
            //                {
            //                    IncludeAllCategories = false
            //                }));
            //        }
            //        else
            //        {
            //            source.Request.EvidenceNoteIds.Remove(evidenceNoteData.Id);
            //            source.Request.DeselectedEvidenceNoteIds.Add(evidenceNoteData.Id);
            //        }
            //    }
            //}

            foreach (var evidenceNoteData in source.AvailableNotes.Results)
            {
                //if (evidenceNoteData.EvidenceTonnageData.Any(e => source.Categories.Contains(e.CategoryId.ToInt())))
                //{
                    model.EvidenceNotesDataListPaged.Add(Mapper.Map<EvidenceNoteRowViewModel>(evidenceNoteData));
                //}
                //else
                //{
                    //source.Request.EvidenceNoteIds.Remove(evidenceNoteData.Id);
                    //source.Request.DeselectedEvidenceNoteIds.Add(evidenceNoteData.Id);
                //}   
            }

            model.EvidenceNotesDataListPaged = 
                model.EvidenceNotesDataListPaged.ToPagedList(source.PageNumber - 1, source.PageSize, source.AvailableNotes.NoteCount) as PagedList<EvidenceNoteRowViewModel>;

            model.SearchRef = source.SearchRef;
            model.PageNumber = source.PageNumber;

            return model;
        }
    }
}