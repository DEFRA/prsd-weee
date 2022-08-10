namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            
            var model = MapBaseProperties(source);

            foreach (var evidenceNoteData in source.Notes.Results.OrderByDescending(e => e.Reference))
            {
                var selected = false;
                if (source.TransferEvidenceNoteData != null)
                {
                    selected = source.TransferEvidenceNoteData.TransferEvidenceNoteTonnageData.Any(t =>
                        t.OriginalNoteId == evidenceNoteData.Id);
                }
                model.SelectedEvidenceNotePairs.Add(new GenericControlPair<Guid, bool>(evidenceNoteData.Id, selected));
            }

            if (source.SessionEvidenceNotesId != null)
            {
                model.SelectedEvidenceNotePairs.Where(c => source.SessionEvidenceNotesId.Contains(c.Key)).ToList()
                           .ForEach(c => c.Value = true);

                model.SelectedEvidenceNotePairs = ReorderAndGetSelectedEvidenceIds(model);
            }

            if (source.TransferEvidenceNoteData != null)
            {
                model.SelectedEvidenceNotePairs = ReorderAndGetSelectedEvidenceIds(model);
            }

            return model;
        }

        private List<GenericControlPair<Guid, bool>> ReorderAndGetSelectedEvidenceIds(TransferEvidenceNotesViewModel model)
        {
            var selectedNoteIds = model.SelectedEvidenceNotePairs
                 .Where(s => s.Value)
                 .Select(s1 => s1.Key).ToList();

            // order list by the selected notes first (in desc ref order) and then the rest of the notes in ref desc
            var formattedList = new List<ViewEvidenceNoteViewModel>();
            formattedList.AddRange(model.EvidenceNotesDataList.Where(e => selectedNoteIds.Contains(e.Id)).OrderByDescending(e => e.Reference).ToList());
            formattedList.AddRange(model.EvidenceNotesDataList.Where(e => !selectedNoteIds.Contains(e.Id)).OrderByDescending(e => e.Reference).ToList());
            model.EvidenceNotesDataList = formattedList;

            // reorder the selected evidence note ids to ensure they match the list of notes
            return model.EvidenceNotesDataList.Select(e =>
                new GenericControlPair<Guid, bool>(e.Id, selectedNoteIds.Contains(e.Id))).ToList();
        }
    }
}