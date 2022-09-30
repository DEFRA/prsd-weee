namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using System.Collections.Generic;
    using Web.ViewModels.Shared;

    public class EvidenceNoteHistoryDataViewModelMap : IMap<List<EvidenceNoteHistoryData>, IList<EvidenceNoteRowViewModel>>
    {
        public IList<EvidenceNoteRowViewModel> Map(List<EvidenceNoteHistoryData> source)
        {
            var model = new List<EvidenceNoteRowViewModel>();

            foreach (var historyData in source)
            {
                model.Add(new EvidenceNoteRowViewModel()
                {
                    Id = historyData.Id,
                    ReferenceId = historyData.Reference,
                    TransferredTo = historyData.TransferredTo,
                    Type = historyData.Type,
                    Status = historyData.Status,
                    SubmittedDate = historyData.SubmittedDate
                });
            }

            return model;
        }
    }
}