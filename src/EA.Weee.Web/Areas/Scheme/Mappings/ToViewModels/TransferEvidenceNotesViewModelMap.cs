namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Shared;
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

            foreach (var evidenceNoteData in source.Notes)
            {
                var selected = false;
                if (source.TransferEvidenceNoteData != null)
                {
                    selected = source.TransferEvidenceNoteData.TransferEvidenceNoteTonnageData.Any(t =>
                        t.OriginalNoteId == evidenceNoteData.Id);
                }
                model.SelectedEvidenceNotePairs.Add(new GenericControlPair<Guid, bool>(evidenceNoteData.Id, selected));
            }

            return model;
        }
    }
}