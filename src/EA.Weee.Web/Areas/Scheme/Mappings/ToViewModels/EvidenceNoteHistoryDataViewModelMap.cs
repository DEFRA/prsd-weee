namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using System.Collections.Generic;

    public class EvidenceNoteHistoryDataViewModelMap : IMap<EvidenceNoteHistoryDataViewModelMapTransfer, IList<EvidenceNoteHistoryViewModel>>
    {
        public IList<EvidenceNoteHistoryViewModel> Map(EvidenceNoteHistoryDataViewModelMapTransfer source)
        {
            var model = new List<EvidenceNoteHistoryViewModel>();

            foreach (var historyData in source.NoteHistoryData)
            {
                model.Add(new EvidenceNoteHistoryViewModel(historyData.Id, historyData.Reference, historyData.TransferredTo, historyData.Type, historyData.Status, historyData.SubmittedDate));
            }

            return model;
        }
    }
}